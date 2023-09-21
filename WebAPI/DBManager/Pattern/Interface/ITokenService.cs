using System.Security.Claims;

namespace DBManager.Pattern.Interface {
    public interface ITokenService {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
