using WebApi.Entities;
using WebApi.Models.Users;

namespace WebApi.Services
{
    public interface IUserService
    {
        LoginResponse Login(LoginRequest model, string ipAddress);
        void Register(User user);
        LoginResponse RefreshToken(string token, string ipAddress);
        User GetById(int id);
        List<User> GetAll();
    }
}
