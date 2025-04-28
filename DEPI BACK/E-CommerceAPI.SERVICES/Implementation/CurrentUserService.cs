using E_CommerceAPI.ENTITES.Helpers;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class CurrentUserService : ResponseHandler, ICurrentUserService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(UserManager<ApplicationUser> userManager,
                                   IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<Guid>> GetUserId()
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
                    .SingleOrDefault(claim => claim.Type == nameof(UserClaimModel.Id))?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized<Guid>("User ID not found or invalid.");
                }

                return Success(userId);
            }
            catch (Exception ex)
            {
                // Log exception here
                return BadRequest<Guid>("An error occurred while fetching the user ID.");
            }
        }

        public async Task<Response<ApplicationUser>> GetUserAsync()
        {
            var userIdResponse = await GetUserId();

            if (!userIdResponse.Succeeded)
            {
                return Unauthorized<ApplicationUser>("User is not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(userIdResponse.Data.ToString());

            if (user == null)
            {
                return Unauthorized<ApplicationUser>("User not found.");
            }

            return Success(user);
        }

        public async Task<Response<List<string>>> GetCurrentUserRolesAsync()
        {
            var userResponse = await GetUserAsync();

            if (!userResponse.Succeeded)
            {
                return Unauthorized<List<string>>("User not authenticated or not found.");
            }

            var roles = await _userManager.GetRolesAsync(userResponse.Data);
            return Success(roles.ToList());
        }

    }
}
