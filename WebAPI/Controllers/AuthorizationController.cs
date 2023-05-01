using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase {
        private readonly ILogger<AuthorizationController> _logger; //для логов
        private readonly ITokenService _tokenService;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<User> _userRepository;
        public AuthorizationController(ILogger<AuthorizationController> logger, 
                                       IServiceProvider serviceProvider, 
                                       ITokenService tokenService) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _userRepository = _unitOfWork.GetRepository<User>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel) {
            _logger.LogInformation("/api/Authorization : post request");

            if (loginModel is null) {
                return BadRequest("Invalid client request");
            }
            User? user = _userRepository.GetFirstOrDefault(predicate: x => x.Email == loginModel.Email && x.Password == loginModel.Password,
                                                           include: i => i.Include(x => x.Role));
            if (user is null)
                return Unauthorized();
            if (user.IsLocked) {
                return StatusCode(403);
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.SerialNumber, user!.Id.ToString()),
                new Claim(ClaimTypes.Email, user!.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Role, user.Role!.Name)
            };
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddYears(1);
            
            _userRepository.Update(user);
            _unitOfWork.SaveChanges();

            return Ok(new {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
