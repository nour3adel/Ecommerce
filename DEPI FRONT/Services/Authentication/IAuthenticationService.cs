using Ecommerce.Frontend.Models.Authentication;
using Ecommerce.Frontend.Models.Common;

namespace Ecommerce.Frontend.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<Response<LoginResponse>> LoginAsync(LoginDto loginDto);
        Task<Response<string>> RegisterAsync(RegisterDto registerDto);
        Task<Response<string>> SendResetPasswordAsync(string email);
        Task<Response<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<Response<string>> ConfirmResetPassword(string code, string email);

    }
}
