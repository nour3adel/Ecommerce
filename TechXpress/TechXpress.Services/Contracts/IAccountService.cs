using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Services.DTOs.UserDtos;

namespace TechXpress.Services.Contracts
{
    public interface IAccountService
    {
        Task<bool> Register(UserRegisterDto user); // async
    }
}
