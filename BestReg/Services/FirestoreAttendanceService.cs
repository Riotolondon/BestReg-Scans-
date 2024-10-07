//using BestReg.Data;
//using Google.Cloud.Firestore;

//namespace BestReg.Services
//{
//    public class FirestoreAttendanceService
//    {
//        private readonly FirestoreDb _firestoreDb;

//        public FirestoreAttendanceService(FirestoreDb firestoreDb)
//        {
//            _firestoreDb = firestoreDb;
//        }

//        public async Task SaveOrUpdateAttendanceRecordAsync(AttendanceRecord record)
//        {
//            var docRef = _firestoreDb.Collection("attendanceRecords").Document(record.Id.ToString());
//            await docRef.SetAsync(record, SetOptions.MergeAll);
//        }
//    }
//}
