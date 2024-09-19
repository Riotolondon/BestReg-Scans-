using FirebaseAdmin.Auth;
using System.Threading.Tasks;

namespace BestReg.Services
{
    public class FirebaseTokenValidator : IFirebaseTokenValidator
    {
        public async Task<bool> ValidateTokenAsync(string idToken)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return decodedToken != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
