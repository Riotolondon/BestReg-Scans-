using BestReg.Data;


namespace BestReg.Data
{
public class AttendanceRecord
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime AttendanceDate { get; set; }

    public DateTime? BusCheckInHome { get; set; }
    public DateTime? BusCheckOutSchool { get; set; }

    public DateTime? SchoolCheckIn { get; set; }
    public DateTime? SchoolCheckOut { get; set; }

    public DateTime? BusCheckInSchool { get; set; }
    public DateTime? BusCheckOutHome { get; set; }

    public ApplicationUser User { get; set; }
}
}

