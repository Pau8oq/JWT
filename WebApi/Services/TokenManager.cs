using IdentityModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Entities;
using WebApi.Model;

namespace WebApi.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly JWTAuthOptions _jWTAuthOptions;
        public TokenManager(IOptions<JWTAuthOptions> jWTAuthOptions)
        {
            _jWTAuthOptions = jWTAuthOptions.Value;
        }


        public string GenerateJWTToken(User user)
        {
            var secret = _jWTAuthOptions.Secret;
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

            var issuer = _jWTAuthOptions.Issuer;
            var subject = _jWTAuthOptions.Subject;
            var audience = _jWTAuthOptions.Audience;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtClaimTypes.Subject, subject)
                }),
                Expires = DateTime.UtcNow.AddSeconds(10),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool ValidateJWTToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var secret = _jWTAuthOptions.Secret;
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

            var issuer = _jWTAuthOptions.Issuer;
            var audience = _jWTAuthOptions.Audience;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = securityKey
                }, out SecurityToken securityToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token);

            var stringClaimValue = securityToken.Claims.First(c => c.Type == claimType).Value;

            return stringClaimValue;
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {

            using var rnd =  RandomNumberGenerator.Create();
            var randomBytes = new byte[64];
            rnd.GetBytes(randomBytes);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddSeconds(30),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;
        }
    }
}
