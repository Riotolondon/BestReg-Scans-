using BestReg.Data;

public class QrCodeScanViewModel
{
    // Add properties if needed
}

public class CheckInRecord
{
    public int Id { get; set; } // Primary key
    public string UserId { get; set; } // Foreign key to ApplicationUser
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; } // Nullable for check-out time
    public bool IsCheckedOut { get; set; }

    public ApplicationUser User { get; set; } // Navigation property
}

public class AttendanceRecord
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public DateTime AttendanceDate { get; set; }
}