using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebAPI.Filter;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger; //для логов
        private readonly ITokenService _tokenService;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<User> _userRepository;
        public TokenController(ILogger<TokenController> logger,
                               IServiceProvider serviceProvider,
                               ITokenService tokenService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _userRepository = _unitOfWork.GetRepository<User>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger.LogDebug("TokenController", "NLog injected into TokenController");
        }
        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] Token token)
        {
            try
            {
                _logger.LogInformation("POST REQUEST");
                if(token.RefreshToken is null)
                    return BadRequest("Invalid client request");
                User? user = _userRepository.GetFirstOrDefault(predicate: x => x.RefreshToken == token.RefreshToken,
                                                         include: i => i.Include(x => x.Role));
                if(user is null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                    return BadRequest("Invalid client request");
                if(user.IsLocked)
                {
                    return StatusCode(403);
                }

                var claims = new[]
            {
                new Claim(ClaimTypes.SerialNumber, user!.Id.ToString()), //Хранит Id пользователя
                new Claim(ClaimTypes.Expiration, user!.IsLocked.ToString()), //Хранит статус пользователя
                new Claim(ClaimTypes.Email, user!.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Role, user.Role!.Name)
            };
                var newAccessToken = _tokenService.GenerateAccessToken(claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshToken = newRefreshToken;
                _userRepository.Update(user);
                _unitOfWork.SaveChanges();
                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
        [HttpPost, Authorize]
        [Route("Revoke")]
        //header
        public async Task<IActionResult> Revoke()
        {
            try
            {
                _logger.LogInformation("POST REQUEST");
                var userEmail = User.Claims.First().Value;
                var user = _userRepository.GetFirstOrDefault(predicate: x => x.Name == userEmail);
                if(user == null) return BadRequest();
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                _userRepository.Update(user);
                _unitOfWork.SaveChanges();
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
    }
}
