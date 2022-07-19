using System.Text.Json.Serialization;
using WebApi.Entities;

namespace WebApi.Models.Users
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string JwtToken { get; set; }

        //[JsonIgnore]
        //цей аттрибут не сереалізує поле коли ми віддіємо respoce 
        //RefreshToken ми вставляємо в cookies
        //поправка, всетаки вручну вставляємо на клієнті
        public string RefreshToken { get; set; }

        public LoginResponse(User user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Email = user.Email;
            UserName = user.UserName; 
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
