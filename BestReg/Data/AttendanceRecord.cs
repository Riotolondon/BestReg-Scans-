public class AttendanceRecord
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
}
