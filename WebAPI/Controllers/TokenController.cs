using DBManager;
using DBManager.Pattern.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase {
        private readonly ILogger<TokenController> _logger; //для логов
        private readonly ITokenService _tokenService;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<User> _userRepository;
        public TokenController(ILogger<TokenController> logger,
                               IServiceProvider serviceProvider, 
                               ITokenService tokenService) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _userRepository = _unitOfWork.GetRepository<User>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        [HttpPost]
        [Route("Refresh")]
        //body
        public IActionResult Refresh([FromBody]Token token) {
            if (token is null)
                return BadRequest("Invalid client request");
            string accessToken = token.AccessToken!;
            string refreshToken = token.RefreshToken!;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userEmail = principal.Claims.First().Value; //get email user
            var user = _userRepository.GetFirstOrDefault(predicate: x => x.Email == userEmail);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");
            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            _unitOfWork.SaveChanges();
            return Ok(new {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpPost, Authorize]
        [Route("Revoke")]
        //header
        public IActionResult Revoke() {
            var userEmail = User.Claims.First().Value;
            var user = _userRepository.GetFirstOrDefault(predicate: x => x.Name == userEmail);
            if (user == null) return BadRequest();
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            _userRepository.Update(user);
            _unitOfWork.SaveChanges();
            return NoContent();
        }
    }
}
