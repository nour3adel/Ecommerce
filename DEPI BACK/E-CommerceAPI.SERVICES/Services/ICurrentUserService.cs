using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface ICurrentUserService
    {
        public Task<Response<ApplicationUser>> GetUserAsync();
        public Task<Response<Guid>> GetUserId();
        public Task<Response<List<string>>> GetCurrentUserRolesAsync();

    }
}
