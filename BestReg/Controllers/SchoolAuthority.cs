using BestReg.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace BestReg.Controllers
{
    [Authorize(Roles = "SchoolAuthority")]
    public class SchoolAuthorityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SchoolAuthorityController> _logger;
        private readonly IEmailService _emailService;

        public SchoolAuthorityController(ApplicationDbContext context, ILogger<SchoolAuthorityController> logger, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService; 
        }

        // GET: SchoolAuthority/Index
        public async Task<IActionResult> Index(string error = null, string success = null)
        {
            ViewBag.Error = error;
            ViewBag.Success = success;

            var today = DateTime.Now.Date;
            var recentActivity = await _context.AttendanceRecords.Where(a => a.AttendanceDate == today).OrderByDescending(a => a.SchoolCheckIn ?? a.SchoolCheckOut ?? DateTime.MinValue).Take(10).Include(a => a.User).ToListAsync();

            return View(recentActivity);
        }

      
        [HttpPost]
        public async Task<IActionResult> SchoolCheckIn(string qrCodeData)
        {
            return await HandleSchoolCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.SchoolCheckIn = DateTime.Now, "Check-in");
        }

        
        [HttpPost]
        public async Task<IActionResult> SchoolCheckOut(string qrCodeData)
        {
            return await HandleSchoolCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.SchoolCheckOut = DateTime.Now, "Check-out");
        }

        // Helper method to handle both check-in and check-out
        private async Task<IActionResult> HandleSchoolCheckInOut(string qrCodeData, DateTime now, Action<AttendanceRecord> updateAction, string actionType)
        {
            if (string.IsNullOrWhiteSpace(qrCodeData))
            {
                _logger.LogWarning("QR Code data is empty.");
                return RedirectToAction("Index", new { error = "Invalid QR Code data." });
            }

            // Find the user by their IDNumber (scanned from the QR code)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.IDNumber == qrCodeData);

            if (user == null)
            {
                _logger.LogWarning($"User with IDNumber {qrCodeData} not found.");
                return RedirectToAction("Index", new { error = "User not found." });
            }

          
            var today = now.Date;

            // Check if an attendance record already exists for the user today
            var attendanceRecord = await _context.AttendanceRecords.FirstOrDefaultAsync(a => a.UserId == user.Id && a.AttendanceDate == today);
            if (attendanceRecord == null)
            {
                attendanceRecord = new AttendanceRecord
                {
                    UserId = user.Id,
                    AttendanceDate = today
                };
                _context.AttendanceRecords.Add(attendanceRecord);
            }

            // Update the attendance record (either check-in or check-out)
            updateAction(attendanceRecord);
            await _context.SaveChangesAsync();

            // Send email notification to parent
            var parentEmail = user.Email;
            if (!string.IsNullOrEmpty(parentEmail))
            {
                var emailSubject = $"Your child has {actionType.ToLower()} at school";
                var emailBody = $"Dear Parent,<br>Your child {user.UserName} has successfully {actionType.ToLower()} at school on {now}.<br>Best Regards,<br>BestReg School";

                var isEmailSent = await _emailService.SendEmailAsync(parentEmail, emailSubject, emailBody);

                if (!isEmailSent)
                {
                    _logger.LogError($"Failed to send email to {parentEmail} for {user.UserName}'s {actionType.ToLower()}.");
                }
                else
                {
                    _logger.LogInformation($"Email sent to {parentEmail} for {user.UserName}'s {actionType.ToLower()}.");
                }
            }


            // Log the action and redirect to the Index page with a success message
            _logger.LogInformation($"{actionType} recorded successfully for {user.UserName}.");
            return RedirectToAction("Index", new { success = $"{actionType} recorded successfully for {user.UserName}." });
        }
    }
}
