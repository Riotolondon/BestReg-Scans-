using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BestReg.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using BestReg.Models;

namespace BestReg.Controllers
{
    [Authorize]
    public class QrCodeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public QrCodeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [HttpGet]
        [Authorize(Roles = "BusDriver, SchoolSecurity")]
        public async Task<IActionResult> CheckInUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var checkInRecord = await _context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.AttendanceDate == DateTime.Today && r.CheckOutTime == null);

            var isCheckingOut = checkInRecord != null;

            var viewModel = new CheckInConfirmationViewModel
            {
                UserId = user.Id,
                UserName = $"{user.FirstName} {user.LastName}",
                CheckInTime = DateTime.Now // Assuming the check-in time is now
            };

            return View("CheckInConfirmation", viewModel);
        }


        // This action displays the QR code scanner interface, restricted to "BusDriver" and "SchoolSecurity" roles
        [Authorize(Roles = "BusDriver, SchoolSecurity")]
        public IActionResult ScanQrCode()
        {
            return View();
        }

        // This action handles the scanned QR code data and processes the check-in or check-out
        [HttpPost]
        [Authorize(Roles = "BusDriver, SchoolSecurity")]
        public async Task<IActionResult> GetUserDetailsByQrCode(string qrCodeData)
        {
            if (string.IsNullOrEmpty(qrCodeData))
            {
                return BadRequest("Invalid QR code data.");
            }

            // Assuming the QR code contains a user ID or some unique identifier
            var user = await _userManager.FindByIdAsync(qrCodeData);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check for existing check-in record without a check-out
            var today = DateTime.Today;
            var attendanceRecord = await _context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.AttendanceDate == today && r.CheckOutTime == null);

            if (attendanceRecord == null)
            {
                // No existing record, create a new check-in record
                attendanceRecord = new AttendanceRecord
                {
                    UserId = user.Id,
                    CheckInTime = DateTime.Now,
                    AttendanceDate = today
                };
                _context.AttendanceRecords.Add(attendanceRecord);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully checked in {user.FirstName} {user.LastName} at {attendanceRecord.CheckInTime}.";
            }
            else
            {
                // Existing record found, mark as check-out
                attendanceRecord.CheckOutTime = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully checked out {user.FirstName} {user.LastName} at {attendanceRecord.CheckOutTime}.";
            }

            return RedirectToAction("CheckInConfirmation", new { userId = user.Id });
        }

        // This action confirms the check-in or check-out and displays the result to the user
        public async Task<IActionResult> CheckInConfirmation(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var viewModel = new CheckInConfirmationViewModel
            {
                UserId = user.Id,
                UserName = $"{user.FirstName} {user.LastName}",
                CheckInTime = DateTime.Now // Assuming the check-in time is now
            };

            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(viewModel);
        }
    }
}
