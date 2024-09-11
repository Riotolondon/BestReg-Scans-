using Microsoft.AspNetCore.Identity;

namespace BestReg.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IDNumber { get; set; }
        public string? QrCodeBase64 { get; set; }
     
    }
}
