using DesktopClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopClient.Helpers
{
    public sealed class UserHelper
    {
        private static UserHelper instance;

        private UserHelper()
        {
            User = GetUserFromFile();
            _client = new HttpClient(new AuthenticationDelegatingHandler(new HttpClientHandler()));
        }

        public static UserHelper Instance 
        {
            get 
            {
                if (instance == null)
                    instance = new UserHelper();
                
                return instance;
            } 
        }

        public User User { get; private set; }
        private readonly HttpClient _client;

        public async Task<bool> LogIn(LoginModel model)
        {
            try
            {
                var loginModel = new
                {
                    email = model.Email,
                    password = model.Password
                };

                var json = JsonConvert.SerializeObject(loginModel);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await _client.PostAsync("https://localhost:7265/api/Authentication/login", data))
                {
                    if (!response.IsSuccessStatusCode)
                        return false;

                    using (HttpContent content = response.Content)
                    {
                        string resultString = await content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<LoginResponse>(resultString);

                        if(obj == null) return false;

                        var user = new User
                        {
                            Id = obj.Id,
                            Email = obj.Email,
                            Name = obj.UserName,
                            AccesssToken = obj.JwtToken,
                            RefreshToken = obj.RefreshToken
                        };

                        SaveUserLocally(user);

                        User = user;

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var json = JsonConvert.SerializeObject(User.RefreshToken);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using (var response = await _client.PostAsync("https://localhost:7265/api/Authentication/refresh-token", data))
            {
                if (!response.IsSuccessStatusCode)
                    return false;

                using (HttpContent content = response.Content)
                {
                    string resultString = await content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<RefreshTokenResponce>(resultString);

                    if (obj == null) return false;

                    var _user = new User
                    {
                        Id = obj.Id,
                        Email = obj.Email,
                        Name = obj.UserName,
                        AccesssToken = obj.JwtToken,
                        RefreshToken = obj.RefreshToken
                    };

                    SaveUserLocally(_user);

                    return true;
                }
            }
        }


        public async Task<List<UserEntity>> GetAll()
        {
            using (var response = await _client.GetAsync("https://localhost:7265/api/Authentication/get-all"))
            {
                if (!response.IsSuccessStatusCode)
                    return null;

                using (HttpContent content = response.Content)
                {
                    string resultString = await content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserEntity>>(resultString);

                    return users;
                }
            }
        }

        public void LogOut()
        {
            File.Delete("user.txt");
            User = null;
        }

        private void SaveUserLocally(User user)
        { 
            if (user == null) return;

            var json = JsonConvert.SerializeObject(user);

            File.WriteAllText("user.txt", json);
        }

        private User GetUserFromFile(string path = "user.txt")
        {
            if (File.Exists(path))
            {
                var strJson = File.ReadAllText("user.txt");

                return JsonConvert.DeserializeObject<User>(strJson);
            }
           
            return null;
        }

        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            try
            {
                var registerModel = new
                {
                    name = model.UserName,
                    email = model.Email,
                    password = model.Password
                };

                var json = JsonConvert.SerializeObject(registerModel);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await _client.PostAsync("https://localhost:7265/api/Authentication/register", data))
                {
                    if (response.IsSuccessStatusCode)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public event EventHandler LoginExpiredEvent;

        public void LoginExpired()
        {
            LogOut();
            this.LoginExpiredEvent.Invoke(this, EventArgs.Empty);
        }
    }
}
