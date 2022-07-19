using WebApi.Entities;

namespace WebApi.Services
{
    public interface ITokenManager
    {
        string GenerateJWTToken(User user);
        bool ValidateJWTToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
        public string GetClaim(string token, string claimType);
    }
}
