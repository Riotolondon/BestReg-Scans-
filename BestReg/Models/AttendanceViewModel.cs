namespace BestReg.Models
{
    public class AttendanceViewModel
    {
        public IEnumerable<BusDriverData> BusDriverRecords { get; set; }
        public IEnumerable<SchoolAuthorityData> SchoolAuthorityRecords { get; set; }
        public IEnumerable<HistoricalAttendanceRecord> HistoricalRecords { get; set; }
    }

    public class BusDriverData
    {
        public string UserName { get; set; }
        public DateTime? BusCheckInHome { get; set; }
        public DateTime? BusCheckOutSchool { get; set; }
        public DateTime? BusCheckInSchool { get; set; }
        public DateTime? BusCheckOutHome { get; set; }
    }

    public class SchoolAuthorityData
    {
        public string UserName { get; set; }
        public DateTime? SchoolCheckIn { get; set; }
        public DateTime? SchoolCheckOut { get; set; }
    }
    public class HistoricalAttendanceRecord
    {
        public string UserName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? BusCheckInHome { get; set; }
        public DateTime? BusCheckOutSchool { get; set; }
        public DateTime? SchoolCheckIn { get; set; }
        public DateTime? SchoolCheckOut { get; set; }
        public DateTime? BusCheckInSchool { get; set; }
        public DateTime? BusCheckOutHome { get; set; }
    }
}
