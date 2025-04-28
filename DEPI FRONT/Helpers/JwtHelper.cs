using Ecommerce.Frontend.Models.Dashboard;
using System.IdentityModel.Tokens.Jwt;

namespace Ecommerce.Frontend.Helpers
{


    public static class JwtHelper
    {
        /// <summary>
        /// Decodes the JWT and returns the JwtSecurityToken object.
        /// </summary>
        private static JwtSecurityToken DecodeToken(string? accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken), "Access token cannot be null or empty.");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                return handler.ReadJwtToken(accessToken);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid or expired JWT.", nameof(accessToken), ex);
            }
        }

        /// <summary>
        /// Retrieves the username from the JWT.
        /// </summary>
        public static string? GetUsernameFromToken(string? accessToken)
        {
            var token = DecodeToken(accessToken);
            return token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        }

        /// <summary>
        /// Retrieves the email address from the JWT.
        /// </summary>
        public static string? GetEmailFromToken(string? accessToken)
        {
            var token = DecodeToken(accessToken);
            return token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
        }

        /// <summary>
        /// Retrieves the user's role from the JWT.
        /// </summary>
        public static string? GetRoleFromToken(string? accessToken)
        {
            var token = DecodeToken(accessToken);
            return token.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
        }

        /// <summary>
        /// Retrieves the user's ID from the JWT.
        /// </summary>
        public static string? GetUserIdFromToken(string? accessToken)
        {
            var token = DecodeToken(accessToken);
            return token.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        }

        /// <summary>
        /// Retrieves the phone number from the JWT.
        /// </summary>
        public static string? GetPhoneNumberFromToken(string? accessToken)
        {
            var token = DecodeToken(accessToken);
            return token.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
        }

        /// <summary>
        /// Retrieves all claims from the JWT as a dictionary.
        /// </summary>
        public static Dictionary<string, string> GetAllClaimsFromToken(string? accessToken)
        {
            var token = DecodeToken(accessToken);
            return token.Claims.ToDictionary(c => c.Type, c => c.Value);
        }

        public static JwtUserInfo GetUserInfoFromToken(string accessToken)
        {
            var token = DecodeToken(accessToken);

            return new JwtUserInfo
            {
                Username = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value,
                Email = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value,
                Role = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value,
                UserId = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value,
                PhoneNumber = token.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value
            };
        }

    }
}
