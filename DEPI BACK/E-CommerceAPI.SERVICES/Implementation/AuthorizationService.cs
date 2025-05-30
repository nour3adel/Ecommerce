﻿using E_CommerceAPI.ENTITES.DTOs.AutherizationDTOs;
using E_CommerceAPI.ENTITES.Helpers;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class AuthorizationService : IAuthorizationService
    {
        #region Fields
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructors
        public AuthorizationService(RoleManager<IdentityRole> roleManager,
                                    UserManager<ApplicationUser> userManager,
                                    IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;

        }


        #endregion

        #region handle Functions
        public async Task<string> AddRoleAsync(string roleName)
        {
            var identityRole = new IdentityRole();
            identityRole.Name = roleName;
            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
                return "Success";
            return "Failed";
        }

        public async Task<bool> IsRoleExistByName(string roleName)
        {
            //var role=await _roleManager.FindByNameAsync(roleName);
            //if(role == null) return false;
            //return true;
            return await _roleManager.RoleExistsAsync(roleName);
        }
        public async Task<string> EditRoleAsync(EditRoleRequest request)
        {
            //check role is exist or not
            var role = await _roleManager.FindByIdAsync(request.Id.ToString());
            if (role == null)
                return "notFound";
            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded) return "Success";
            var errors = string.Join("-", result.Errors);
            return errors;
        }

        public async Task<string> DeleteRoleAsync(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return "NotFound";
            //Chech if user has this role or not
            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            //return exception 
            if (users != null && users.Count() > 0) return "Used";
            //delete
            var result = await _roleManager.DeleteAsync(role);
            //success
            if (result.Succeeded) return "Success";
            //problem
            var errors = string.Join("-", result.Errors);
            return errors;
        }

        public async Task<bool> IsRoleExistById(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return false;
            else return true;
        }

        public async Task<List<IdentityRole>> GetRolesList()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole> GetRoleById(int id)
        {
            return await _roleManager.FindByIdAsync(id.ToString());
        }

        public async Task<ManageUserRolesResult> ManageUserRolesData(ApplicationUser user)
        {
            var response = new ManageUserRolesResult();
            var rolesList = new List<UserRoles>();
            //Roles
            var roles = await _roleManager.Roles.ToListAsync();
            response.UserId = user.Id;
            foreach (var role in roles)
            {
                var userrole = new UserRoles();
                userrole.Id = role.Id;
                userrole.Name = role.Name;
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userrole.HasRole = true;
                }
                else
                {
                    userrole.HasRole = false;
                }
                rolesList.Add(userrole);
            }
            response.userRoles = rolesList;
            return response;
        }

        public async Task<string> UpdateUserRoles(UpdateUserRolesRequest request)
        {
            var transact = await _unitOfWork.Carts.BeginTransactionAsync();
            try
            {
                //Get User
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return "UserIsNull";
                }
                //get user Old Roles
                var userRoles = await _userManager.GetRolesAsync(user);
                //Delete OldRoles
                var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!removeResult.Succeeded)
                    return "FailedToRemoveOldRoles";
                var selectedRoles = request.userRoles.Where(x => x.HasRole == true).Select(x => x.Name);

                //Add the Roles HasRole=True
                var addRolesresult = await _userManager.AddToRolesAsync(user, selectedRoles);
                if (!addRolesresult.Succeeded)
                    return "FailedToAddNewRoles";
                await transact.CommitAsync();
                //return Result
                return "Success";
            }
            catch (Exception ex)
            {
                await transact.RollbackAsync();
                return "FailedToUpdateUserRoles";
            }
        }
        public async Task<ManageUserClaimsResult> ManageUserClaimData(ApplicationUser user)
        {
            var response = new ManageUserClaimsResult();
            var usercliamsList = new List<UserClaims>();
            response.UserId = user.Id;
            //Get USer Claims
            var userClaims = await _userManager.GetClaimsAsync(user);

            foreach (var claim in ClaimsStore.claims)
            {
                var userclaim = new UserClaims();
                userclaim.Type = claim.Type;
                if (userClaims.Any(x => x.Type == claim.Type))
                {
                    userclaim.Value = true;
                }
                else
                {
                    userclaim.Value = false;
                }
                usercliamsList.Add(userclaim);
            }
            response.userClaims = usercliamsList;
            //return Result
            return response;
        }
        public async Task<string> UpdateUserClaims(UpdateUserClaimsRequest request)
        {
            var transact = await _unitOfWork.Carts.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return "UserIsNull";
                }
                //remove old Claims
                var userClaims = await _userManager.GetClaimsAsync(user);
                var removeClaimsResult = await _userManager.RemoveClaimsAsync(user, userClaims);
                if (!removeClaimsResult.Succeeded)
                    return "FailedToRemoveOldClaims";
                var claims = request.userClaims.Where(x => x.Value == true).Select(x => new Claim(x.Type, x.Value.ToString()));

                var addUserClaimResult = await _userManager.AddClaimsAsync(user, claims);
                if (!addUserClaimResult.Succeeded)
                    return "FailedToAddNewClaims";

                await transact.CommitAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                await transact.RollbackAsync();
                return "FailedToUpdateClaims";
            }
        }
        #endregion
    }
}
