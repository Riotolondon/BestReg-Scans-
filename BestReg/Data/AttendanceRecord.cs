using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;

namespace BestReg.Data
{
    [FirestoreData]
    public class AttendanceRecord
    {
        [Key]
        [FirestoreDocumentId]
        public int Id { get; set; }

        [Required]
        [FirestoreProperty]
        public string UserId { get; set; }

        [Required]
        [FirestoreProperty]
        public DateTime AttendanceDate { get; set; }

        [FirestoreProperty]
        public DateTime? BusCheckInHome { get; set; }

        [FirestoreProperty]
        public DateTime? BusCheckOutSchool { get; set; }

        [FirestoreProperty]
        public DateTime? SchoolCheckIn { get; set; }

        [FirestoreProperty]
        public DateTime? SchoolCheckOut { get; set; }

        [FirestoreProperty]
        public DateTime? BusCheckInSchool { get; set; }

        [FirestoreProperty]
        public DateTime? BusCheckOutHome { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public Dictionary<string, object> ToFirestoreDocument()
        {
            return new Dictionary<string, object>
            {
                ["id"] = Id,
                ["userId"] = UserId,
                ["attendanceDate"] = AttendanceDate,
                ["busCheckInHome"] = BusCheckInHome,
                ["busCheckOutSchool"] = BusCheckOutSchool,
                ["schoolCheckIn"] = SchoolCheckIn,
                ["schoolCheckOut"] = SchoolCheckOut,
                ["busCheckInSchool"] = BusCheckInSchool,
                ["busCheckOutHome"] = BusCheckOutHome
            };
        }
    }
}
