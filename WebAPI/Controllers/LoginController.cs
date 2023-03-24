using DBManager.Pattern;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase {
        private readonly ILogger<LoginController> _logger; //для логов
        private IConfiguration _config; //для токенов
        private UnitOfWork _unitOfWork; //для БД
        public LoginController(ILogger<LoginController> logger, UnitOfWork unitOfWork, IConfiguration config) {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _config = config;

        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin) {
            _unitOfWork.Users.Create(new User() { Email = "danik@popopop.com", Id = 1, Name = "user", Role = new Role() { Id = 1, Name = "User" } });
            _unitOfWork.Users.Save();
            var user = Authenticate(userLogin);

            if (user != null) {
                var token = Generate(user);
                return Ok(token);
            }

            return NotFound("User not found");
        }
        private string Generate(User user) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserLogin.Login),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private User Authenticate(UserLogin userLogin) {
            return new User() { };
        }
    }
}
