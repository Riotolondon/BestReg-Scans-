using BestReg.Data;
using BestReg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Ensure this is included
using System.Security.Claims;
using System.Threading.Tasks;

namespace BestReg.Controllers
{
    [Authorize(Roles = "Student")]
    public class ParentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewQrCode()
        {
            // Implementation to show QR code
            return View();
        }

        public async Task<IActionResult> ViewChildData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var busDriverData = await _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a => a.UserId == userId && a.AttendanceDate == DateTime.Today)
                .Select(a => new BusDriverData
                {
                    UserName = a.User.UserName,
                    BusCheckInHome = a.BusCheckInHome,
                    BusCheckOutSchool = a.BusCheckOutSchool,
                    BusCheckInSchool = a.BusCheckInSchool,
                    BusCheckOutHome = a.BusCheckOutHome
                })
                .ToListAsync();

            var schoolAuthorityData = await _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a => a.UserId == userId && a.AttendanceDate == DateTime.Today)
                .Select(a => new SchoolAuthorityData
                {
                    UserName = a.User.UserName,
                    SchoolCheckIn = a.SchoolCheckIn,
                    SchoolCheckOut = a.SchoolCheckOut
                })
                .ToListAsync();

            var historicalRecords = await _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AttendanceDate)
                .Select(a => new HistoricalAttendanceRecord
                {
                    UserName = a.User.UserName,
                    AttendanceDate = a.AttendanceDate,
                    BusCheckInHome = a.BusCheckInHome,
                    BusCheckOutSchool = a.BusCheckOutSchool,
                    SchoolCheckIn = a.SchoolCheckIn,
                    SchoolCheckOut = a.SchoolCheckOut,
                    BusCheckInSchool = a.BusCheckInSchool,
                    BusCheckOutHome = a.BusCheckOutHome
                })
                .ToListAsync();

            var viewModel = new AttendanceViewModel
            {
                BusDriverRecords = busDriverData,
                SchoolAuthorityRecords = schoolAuthorityData,
                HistoricalRecords = historicalRecords
            };

            return View(viewModel);
        }
    }
}
