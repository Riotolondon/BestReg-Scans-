using BestReg.Data;

namespace BestReg.Models
{
    public class UserDetailsViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string IDNumber { get; set; }
    }
    public class CheckInConfirmationViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CheckInTime { get; set; }
        public bool IsCheckingOut { get; set; }
    }
}
