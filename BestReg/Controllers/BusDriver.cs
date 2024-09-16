using BestReg.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BestReg.Controllers
{
    [Authorize(Roles = "BusDriver")]
    public class BusDriverController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BusDriverController> _logger;
        private readonly IEmailService _emailService;

        public BusDriverController(ApplicationDbContext context, ILogger<BusDriverController> logger, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Scan QR Code View
       [HttpPost]
        public IActionResult ScanQRCode(string scanType)
        {
            ViewBag.ScanType = scanType;
            return View();
        }

        // Bus Check-in at Home
        [HttpPost]
        public async Task<IActionResult> BusCheckInHome(string qrCodeData)
        {
            return await HandleBusCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.BusCheckInHome = DateTime.Now, "check-in at home");
        }

        // Bus Check-out at School
        [HttpPost]
        public async Task<IActionResult> BusCheckOutSchool(string qrCodeData)
        {
            return await HandleBusCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.BusCheckOutSchool = DateTime.Now, "check-out at school");
        }

        // Bus Check-in at School
        [HttpPost]
        public async Task<IActionResult> BusCheckInSchool(string qrCodeData)
        {
            return await HandleBusCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.BusCheckInSchool = DateTime.Now, "check-in at school");
        }

        // Bus Check-out at Home
        [HttpPost]
        public async Task<IActionResult> BusCheckOutHome(string qrCodeData)
        {
            return await HandleBusCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.BusCheckOutHome = DateTime.Now, "check-out at home");
        }

        // Helper Method to Handle Both Check-in and Check-out
        private async Task<IActionResult> HandleBusCheckInOut(string qrCodeData, DateTime now, Action<AttendanceRecord> updateAction, string actionType)
        {
            if (string.IsNullOrWhiteSpace(qrCodeData))
            {
                _logger.LogWarning("QR Code data is empty.");
                return RedirectToAction("Index", new { error = "Invalid QR Code data." });
            }

            // Fetch User by QR Code Data (IDNumber)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.IDNumber == qrCodeData);

            if (user == null)
            {
                _logger.LogWarning("User with IDNumber {IDNumber} not found.", qrCodeData);
                return RedirectToAction("Index", new { error = "User not found." });
            }

            var today = now.Date;
            var attendanceRecord = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.UserId == user.Id && a.AttendanceDate == today);

            if (attendanceRecord == null)
            {
                // Create a new attendance record if it doesn't exist
                attendanceRecord = new AttendanceRecord
                {
                    UserId = user.Id,
                    AttendanceDate = today
                };
                _context.AttendanceRecords.Add(attendanceRecord);
            }

            // Perform the action (Check-in or Check-out)
            updateAction(attendanceRecord);
            await _context.SaveChangesAsync();

            // Send notification email to parents if applicable
            var parentEmail = user.Email;
            if (!string.IsNullOrEmpty(parentEmail))
            {
                var emailSubject = $"Bus {actionType} notification";
                var emailBody = $"Dear Parent,<br>Your child {user.FirstName+" "+user.LastName} has successfully {actionType} from the bus on {now}.<br>Best Regards,<br>BestReg School Bus";

                var isEmailSent = await _emailService.SendEmailAsync(parentEmail, emailSubject, emailBody);

                if (!isEmailSent)
                {
                    _logger.LogError($"Failed to send email to {parentEmail} for {user.UserName}'s bus {actionType}.");
                }
                else
                {
                    _logger.LogInformation($"Email sent to {parentEmail} for {user.UserName}'s bus {actionType}.");
                }
            }

            return RedirectToAction("Index", new { success = $"{actionType} recorded successfully." });
        }
    }
}
