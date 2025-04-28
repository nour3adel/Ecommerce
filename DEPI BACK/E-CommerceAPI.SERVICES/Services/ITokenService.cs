using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface ITokenService
    {
        Task<(JwtSecurityToken, string)> GenerateJwtToken(ApplicationUser user);
        Task<List<Claim>> GetClaims(ApplicationUser user);
        Task<Response<string>> ValidateToken(string accessToken);
        RefreshToken CreateRefreshToken();
    }
}
