using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Models;
using WebApi.Services;
using WebApi.Entities;

namespace WebApi.Controllers
{
    [ApiController]
    [BearerTokenValidationFilter]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenManager _tokenManager;
        private readonly IUserService _userService;

        public AuthenticationController(ITokenManager tokenManager, IUserService userService)
        {
            _tokenManager = tokenManager;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = _userService.Login(model, IpAddress());
            SetTokenToCookie(response.RefreshToken);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = new User
            {
                Email = model.Email,
                PasswordHash = model.Password,
                UserName = model.Name
            };

            _userService.Register(user);

            return Ok();
        }

        [HttpGet("get-all")]
        [BearerTokenValidationFilter]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();

            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(RefreshTokenRequest model)
        {
            var refreshToken = model.RefreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest();

            var response = _userService.RefreshToken(refreshToken, IpAddress());

            if(response == null)
                return Unauthorized();

            SetTokenToCookie(response.RefreshToken);

            return Ok(response);
        }


        [HttpGet("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok();
        }


        [HttpGet]
        [Route("test-custom-filter")]
        [BearerTokenValidationFilter]
        public IActionResult TestCustomFilter()
        {
            return Ok("some custom text");
        }

        [HttpGet]
        [Route("test-default-authorize")]
        [Authorize]
        public IActionResult TestDefaultAuthorize()
        {
            return Ok("some text");
        }

        [HttpGet("test-excpetion")]
        [AllowAnonymous]
        public IActionResult TestException()
        {
            throw new Exception("some exception");
        }

        private void SetTokenToCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(1),
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}
