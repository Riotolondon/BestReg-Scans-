using Google.Cloud.Firestore;

namespace BestReg.Services
{
    public interface IFirebaseService
    {
        Task AddDocumentAsync<T>(T model) where T : class;
        Task<T> GetDocumentAsync<T>(string documentId) where T : class;
        Task<List<T>> GetAllDocumentsAsync<T>() where T : class;
    }

    public class FirebaseService : IFirebaseService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirebaseService()
        {
            Console.WriteLine("Initializing Firestore...");
            _firestoreDb = FirestoreDb.Create("newchilddb");
            Console.WriteLine("Firestore initialized.");
        }

        public async Task AddDocumentAsync<T>(T model) where T : class
        {
            try
            {
                Console.WriteLine("Adding document...");
                CollectionReference colRef = _firestoreDb.Collection(typeof(T).Name.ToLower());
                await colRef.AddAsync(model);
                Console.WriteLine("Document added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding document: {ex.Message}");
                throw;
            }
        }

        public async Task<T> GetDocumentAsync<T>(string documentId) where T : class
        {
            try
            {
                Console.WriteLine($"Fetching document {documentId}...");
                DocumentReference docRef = _firestoreDb.Collection(typeof(T).Name.ToLower()).Document(documentId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Console.WriteLine("Document found.");
                    return snapshot.ConvertTo<T>();
                }
                Console.WriteLine("Document not found.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching document: {ex.Message}");
                throw;
            }
        }

        public async Task<List<T>> GetAllDocumentsAsync<T>() where T : class
        {
            try
            {
                Console.WriteLine("Fetching all documents...");
                CollectionReference colRef = _firestoreDb.Collection(typeof(T).Name.ToLower());
                QuerySnapshot snapshot = await colRef.GetSnapshotAsync();
                Console.WriteLine("Documents fetched.");
                return snapshot.Documents.Select(doc => doc.ConvertTo<T>()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching documents: {ex.Message}");
                throw;
            }
        }
    }

}
