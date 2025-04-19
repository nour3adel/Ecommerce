using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.Contracts;
using TechXpress.Data.Models;
using TechXpress.Services.Contracts;
using TechXpress.Services.DTOs.UserDtos;

namespace TechXpress.Services.Services
{
    internal class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Register(UserRegisterDto user)
        {
            AppUser appUser = new AppUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,

                Email = user.Email,
                UserName = user.UserName,

            };

            var createResult = await _unitOfWork.UserManager.CreateAsync(appUser,user.Password);
            if (createResult.Succeeded) { 
                // add role here 
                return true;
            }
            return false;
        }
    }
}
