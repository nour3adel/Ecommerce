using E_CommerceAPI.ENTITES.DTOs.AutherizationDTOs;
using E_CommerceAPI.ENTITES.Models;
using Microsoft.AspNetCore.Identity;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IAuthorizationService
    {
        public Task<string> AddRoleAsync(string roleName);
        public Task<bool> IsRoleExistByName(string roleName);
        public Task<string> EditRoleAsync(EditRoleRequest request);
        public Task<string> DeleteRoleAsync(int roleId);
        public Task<bool> IsRoleExistById(int roleId);
        public Task<List<IdentityRole>> GetRolesList();
        public Task<IdentityRole> GetRoleById(int id);
        public Task<ManageUserRolesResult> ManageUserRolesData(ApplicationUser user);
        public Task<string> UpdateUserRoles(UpdateUserRolesRequest request);
        public Task<ManageUserClaimsResult> ManageUserClaimData(ApplicationUser user);
        public Task<string> UpdateUserClaims(UpdateUserClaimsRequest request);
    }
}
