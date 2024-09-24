using BestReg.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestReg.Models
{
    public class AttendanceDataViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AttendanceDataViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var today = DateTime.Now.Date;
            var attendanceData = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate == today)
                .Select(a => new
                {
                    a.BusCheckInHome,
                    a.BusCheckOutSchool,
                    a.BusCheckInSchool,
                    a.BusCheckOutHome,
                    a.User.UserName
                })
                .ToListAsync();
            return View(attendanceData);
        }
    }

}