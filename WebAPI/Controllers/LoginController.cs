using Models;
using DBManager;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using DBManager.Pattern.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase {
        private readonly ILogger<LoginController> _logger; //для логов
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<User> _userRepository;
        private IRepository<Role> _roleRepository;
        private IConfiguration _configuration; //для токенов
        public LoginController(ILogger<LoginController> logger, IServiceProvider serviceProvider, IConfiguration configuration) {
            _logger = logger;
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>();
            _userRepository = _unitOfWork.GetRepository<User>();
            _roleRepository = _unitOfWork.GetRepository<Role>();
            _configuration = configuration;
        }

        [AllowAnonymous]
        [Route("/Token")]
        [HttpPost]
        public IResult Token([FromBody] LoginModel loginModel) {
            User? user = _userRepository.GetFirstOrDefault(predicate: x => x.Email == loginModel.Email && x.Password == loginModel.Password);
            bool IsExistUser = user is not null;
            if (!IsExistUser) {
                return Results.BadRequest(new { message = "Invalid username or password." });
            }
            user!.Role = _roleRepository.GetFirstOrDefault(predicate: x => x.Id == user.RoleId);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user!.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Role, user.Role!.Name)
            };


            var now = DateTime.UtcNow;
            double lifeTime = Convert.ToDouble(_configuration["Jwt:LifeTime"]);
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                                           _configuration["Jwt:Audience"],
                                           claims,
                                           expires: DateTime.Now.AddMinutes(15),
                                           signingCredentials: credentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            user.Token = encodedJwt;
            _userRepository.Update(user);
            _unitOfWork.SaveChanges();
            return Results.Ok(new { message = encodedJwt });
        }
    }
}
