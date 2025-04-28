using E_CommerceAPI.ENTITES.DTOs.AccountDTO;
using E_CommerceAPI.ENTITES.DTOs.UserDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IAccountService
    {
        public Task<Response<LoginResponse>> LoginAsync(LoginDto login);
        public Task<Response<LoginResponse>> GetRefreshToken(string email);
        public Task<Response<string>> RegisterAsync(RegisterDto register);
        public Task<Response<string>> DeleteAccountAsync(LoginDto dto);
        public Task<Response<string>> ChangePassword(PasswordSettingDto password);
        public Task<Response<string>> UpdateProfile(UserDto user, ApplicationUser current);
        public Task<Response<string>> ResetPassword(ResetPasswordDto dto);
        public Task<Response<string>> ConfirmEmail(string userId, string Code);
        public Task<Response<string>> ConfirmResetPassword(string Code, string Email);
        public Task<Response<string>> SendResetPassword(string email);
    }


}
