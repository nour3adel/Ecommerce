using E_CommerceAPI.ENTITES.Helpers;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class TokenService : ResponseHandler, ITokenService
    {

        #region Fields

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signin;
        private readonly JwtSettings _jwtSettings;

        #endregion

        #region Constructor
        public TokenService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signin, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _signin = signin;
            _jwtSettings = jwtSettings;

        }

        #endregion

        #region Generate JwtToken
        public async Task<(JwtSecurityToken, string)> GenerateJwtToken(ApplicationUser user)
        {
            var claims = await GetClaims(user);
            var jwtToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.Now.AddDays(_jwtSettings.AccessTokenExpireDate),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)), SecurityAlgorithms.HmacSha256Signature));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return (jwtToken, accessToken);
        }
        #endregion

        #region Get All Claims
        public async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(nameof(UserClaimModel.PhoneNumber), user.PhoneNumber ?? ""),
                new Claim(nameof(UserClaimModel.Id), user.Id.ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);
            return claims;
        }
        #endregion

        #region Validate Token
        public async Task<Response<string>> ValidateToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuers = new[] { _jwtSettings.Issuer },
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
                ValidAudience = _jwtSettings.Audience,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidateLifetime = false // Don't validate lifetime here; we handle expiration separately.
            };

            try
            {
                // Validate the token
                var validator = handler.ValidateToken(accessToken, parameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    var expiration = jwtToken.ValidTo; // Get the token's expiration time (UTC)
                    var currentUtcTime = DateTime.UtcNow;

                    if (currentUtcTime >= expiration)
                    {
                        return BadRequest<string>("TokenExpired");
                    }

                    return Success($"Token is valid and will expire on: {expiration:O} (UTC).");
                }

                return BadRequest<string>("InvalidToken");
            }
            catch (Exception ex)
            {
                return BadRequest<string>($"Validation failed: {ex.Message}");
            }
        }




        #endregion

        #region Create Refresh Token
        public RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            var randomNumberGenerate = RandomNumberGenerator.Create();
            randomNumberGenerate.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddHours(6),
                CreatedOn = DateTime.UtcNow
            };
        }
        #endregion
    }

}
