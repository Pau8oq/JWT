using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;

namespace WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly WebDbContext _context;
        private readonly ITokenManager _tokenManager;

        public UserService(WebDbContext context,
            ITokenManager tokenManager)
        {
            _context = context;
            _tokenManager = tokenManager;
        }


        public User GetById(int id)
        {
            var user = _context.Users.Find(id);
          
            return user;
        }

        public LoginResponse Login(LoginRequest model, string ipAddress)
        {
            var user = _context.Users.Include(s=>s.RefreshTokens).SingleOrDefault(w => w.Email == model.Email);

            if (user == null || user.PasswordHash != model.Password)
                throw new Exception("User name or password is incorect");

            var jwtToken = _tokenManager.GenerateJWTToken(user);
            var refreshToken = _tokenManager.GenerateRefreshToken(ipAddress);

            user.RefreshTokens.Add(refreshToken);

            RemoveOldRefreshTokens(user);

            _context.Update(user);
            _context.SaveChanges();

            return new LoginResponse(user, jwtToken, refreshToken.Token);

        }

        public LoginResponse RefreshToken(string token, string ipAddress)
        {
            if(string.IsNullOrEmpty(token))
                throw new Exception("Invalid token");

            var user = GetUserByRefreshToken(token);

            var refreshToken = user.RefreshTokens.Single(w => w.Token == token);

            if (refreshToken.IsExpired)
                return null;

            var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            RemoveOldRefreshTokens(user);

            _context.Users.Update(user);
            _context.SaveChanges();

            var jwtToken = _tokenManager.GenerateJWTToken(user);

            return new LoginResponse(user, jwtToken, newRefreshToken.Token);
        }

        private User GetUserByRefreshToken(string token)
        {
            var user = _context.Users.Include(s=>s.RefreshTokens).SingleOrDefault(w => w.RefreshTokens.Any(s => s.Token == token));

            if (user == null)
                throw new Exception("Invalid token");

            return user;
        }

        private void RemoveOldRefreshTokens(User user)
        {
            user.RefreshTokens.RemoveAll(w => !w.IsActive);
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _tokenManager.GenerateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokeByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        public void Register(User user)
        {
            var userExist = _context.Users.Any(w => w.Email == user.Email);

            if (!userExist)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
        }

        public List<User> GetAll()
        {
            var users = new List<User>();

            users = _context.Users.Include(w => w.RefreshTokens).ToList();
            return users;
        }
    }
}
