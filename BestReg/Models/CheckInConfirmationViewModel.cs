namespace BestReg.Models
{
    public class CheckInConfirmationViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? CheckInTime { get; set; } 
        public DateTime? CheckOutTime { get; set; } 
    }
}
