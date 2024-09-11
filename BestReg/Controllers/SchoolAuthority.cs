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

        public SchoolAuthorityController(ApplicationDbContext context, ILogger<SchoolAuthorityController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: SchoolAuthority/Index
        public async Task<IActionResult> Index(string error = null, string success = null)
        {
            ViewBag.Error = error;
            ViewBag.Success = success;

            var today = DateTime.Now.Date;
            var recentActivity = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate == today)
                .OrderByDescending(a => a.SchoolCheckIn ?? a.SchoolCheckOut ?? DateTime.MinValue)
                .Take(10)
                .Include(a => a.User)
                .ToListAsync();

            return View(recentActivity);
        }

        // POST: SchoolAuthority/SchoolCheckIn
        [HttpPost]
        public async Task<IActionResult> SchoolCheckIn(string qrCodeData)
        {
            return await HandleSchoolCheckInOut(qrCodeData, DateTime.Now, (attendance) => attendance.SchoolCheckIn = DateTime.Now, "Check-in");
        }

        // POST: SchoolAuthority/SchoolCheckOut
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

            // Get today's date
            var today = now.Date;

            // Check if an attendance record already exists for the user today
            var attendanceRecord = await _context.AttendanceRecords.FirstOrDefaultAsync(a => a.UserId == user.Id && a.AttendanceDate == today);

            // If no record exists, create a new one
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

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Log the action and redirect to the Index page with a success message
            _logger.LogInformation($"{actionType} recorded successfully for {user.UserName}.");
            return RedirectToAction("Index", new { success = $"{actionType} recorded successfully for {user.UserName}." });
        }
    }
}
