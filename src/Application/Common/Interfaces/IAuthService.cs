using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChallengeApp.Application.Common.Interfaces;

public interface IAuthService
{
    string GenerateRefreshToken();
    string GetNameByToken(string token);
    string GetEmailByToken(string token);
    JwtSecurityToken CreateToken(List<Claim> authClaims, bool isRememberMe);

}