using BestReg.Models; // Ensure you have this using directive for your models
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BestReg.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Entities
        public DbSet<CheckInRecord> CheckInRecords { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    }
}
