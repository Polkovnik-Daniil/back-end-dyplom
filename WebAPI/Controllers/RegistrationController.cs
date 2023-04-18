using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace WebAPI.Controllers {
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class RegistrationController : ControllerBase {
        private readonly ILogger<RegistrationController> _logger; //для логов
        private readonly ITokenService _tokenService;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<User> _userRepository;
        public RegistrationController(ILogger<RegistrationController> logger,
                                       IServiceProvider serviceProvider,
                                       ITokenService tokenService) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _userRepository = _unitOfWork.GetRepository<User>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody] User newUser) {
            if (newUser is null) {
                return BadRequest("Invalid client request");
            }
            User? user = _userRepository.GetFirstOrDefault(predicate: x => x.Email == newUser.Email);
            if (user is not null)
                return BadRequest("User with this email is already registered!");
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, newUser!.Email),
                new Claim(ClaimTypes.GivenName, newUser.Name),
                new Claim(ClaimTypes.Role, "User")
            };
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            newUser.RefreshToken = refreshToken;
            newUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(15);

            _userRepository.Insert(newUser);
            _unitOfWork.SaveChanges();

            return Ok(new {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
