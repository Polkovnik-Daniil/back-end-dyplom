using DBManager;
using DBManager.Pattern;
using DBManager.Pattern.UnitOfWork;
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
        public LoginController(ILogger<LoginController> logger, IConfiguration config) {
            _logger = logger;
            _config = config;
        }

        [AllowAnonymous]
        [HttpGet]
        public void Login() { 
            //var _unitOfWork = Program.ServiceProvider.GetService<IUnitOfWork>();
            //var roleRepository = _unitOfWork?.GetRepository<Role>();
            //var a = roleRepository.GetAll(false).ToList();
            //roleRepository.Insert(new Role() { Name = "User" });
            //_unitOfWork.SaveChanges();
            //if (!_unitOfWork.LastSaveChangesResult.IsOk) {
            //    var defaultColor = Console.ForegroundColor;
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(_unitOfWork.LastSaveChangesResult.Exception.Message);
            //    Console.ForegroundColor = defaultColor;
            //}
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin) {
            //_unitOfWork.Users.Create(new User() { Email = "danik@popopop.com", Id = 1, Name = "user", Role = new Role() { Id = 1, Name = "User" } });
            //_unitOfWork.Users.Save();
            ///
            //IRepository<UserLogin>? userLogin1 = _unitOfWork?.GetRepository<UserLogin>();
            //var user = Authenticate(userLogin);

            //if (user != null) {
            //    var token = Generate(user);
            //    return Ok(token);
            //}
            ///

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
