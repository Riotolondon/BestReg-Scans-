//using Google.Cloud.Firestore;

//namespace BestReg.Services
//{
//    public interface IFirebaseService
//    {
//        Task AddDocumentAsync<T>(T model) where T : class;
//        Task<T> GetDocumentAsync<T>(string documentId) where T : class;
//        Task<List<T>> GetAllDocumentsAsync<T>() where T : class;
//    }

//    public class FirebaseService : IFirebaseService
//    {
//        private readonly FirestoreDb _firestoreDb;

//        public FirebaseService()
//        {
//            _firestoreDb = FirestoreDb.Create("newchilddb");
//        }

//        public async Task AddDocumentAsync<T>(T model) where T : class
//        {
//            CollectionReference colRef = _firestoreDb.Collection(typeof(T).Name.ToLower());
//            await colRef.AddAsync(model);
//        }

//        public async Task<T> GetDocumentAsync<T>(string documentId) where T : class
//        {
//            DocumentReference docRef = _firestoreDb.Collection(typeof(T).Name.ToLower()).Document(documentId);
//            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
//            if (snapshot.Exists)
//            {
//                return snapshot.ConvertTo<T>();
//            }
//            return null;
//        }

//        public async Task<List<T>> GetAllDocumentsAsync<T>() where T : class
//        {
//            CollectionReference colRef = _firestoreDb.Collection(typeof(T).Name.ToLower());
//            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();
//            return snapshot.Documents.Select(doc => doc.ConvertTo<T>()).ToList();
//        }
//    }
//}
