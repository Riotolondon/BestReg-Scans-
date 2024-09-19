namespace BestReg.Services
{
    public interface IFirebaseTokenValidator
    {
        Task<bool> ValidateTokenAsync(string idToken);
    }
}
