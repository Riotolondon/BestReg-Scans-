using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestReg.Data
{
    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }
        //student is picked up and the scanning starts there
        public DateTime? BusCheckInHome { get; set; }
        //student checks out of the bus
        public DateTime? BusCheckOutSchool { get; set; }
        //student checks in the school via the security
        public DateTime? SchoolCheckIn { get; set; }
        //student checks out of school via the security
        public DateTime? SchoolCheckOut { get; set; }
        //student checks in the bus after school 
        public DateTime? BusCheckInSchool { get; set; }
        //student checks out of the bus at home when being dropped off
        public DateTime? BusCheckOutHome { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
