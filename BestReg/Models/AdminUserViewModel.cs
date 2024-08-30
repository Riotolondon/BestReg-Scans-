using BestReg.Data;
using Microsoft.AspNetCore.Identity;

namespace BestReg.Models
{
    public class AdminUserViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
    }

}
