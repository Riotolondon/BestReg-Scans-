using BestReg.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BestReg.Controllers
{
    [Authorize(Roles = "BusDriver")]
    public class BusDriverController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BusDriverController> _logger;

        public BusDriverController(ApplicationDbContext context, ILogger<BusDriverController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ScanQRCode(string scanType)
        {
            ViewBag.ScanType = scanType;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> BusCheckInHome(string qrCodeData)
        {
            return await HandleBusCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.BusCheckInHome = DateTime.Now);
        }

        [HttpGet]
        public async Task<IActionResult> BusCheckOutSchool(string qrCodeData)
        {
            return await HandleBusCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.BusCheckOutSchool = DateTime.Now);
        }

        [HttpGet]
        public async Task<IActionResult> BusCheckInSchool(string qrCodeData)
        {
            return await HandleBusCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.BusCheckInSchool = DateTime.Now);
        }

        [HttpGet]
        public async Task<IActionResult> BusCheckOutHome(string qrCodeData)
        {
            return await HandleBusCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.BusCheckOutHome = DateTime.Now);
        }



        private async Task<IActionResult> HandleBusCheckInOut(string qrCodeData, DateTime now, Action<AttendanceRecord> updateAction)
        {
            if (string.IsNullOrWhiteSpace(qrCodeData))
            {
                _logger.LogWarning("QR Code data is empty.");
                return RedirectToAction("Index", new { error = "Invalid QR Code data." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.IDNumber == qrCodeData);

            if (user == null)
            {
                _logger.LogWarning("User with IDNumber {IDNumber} not found.", qrCodeData);
                return RedirectToAction("Index", new { error = "User not found." });
            }

            var today = DateTime.Now.Date;
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

            updateAction(attendanceRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { success = "Check-in/out recorded successfully." });
        }
    }
}
