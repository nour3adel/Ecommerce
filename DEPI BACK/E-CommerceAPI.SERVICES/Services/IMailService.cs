using E_CommerceAPI.ENTITES.Models;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IMailService
    {
        public Task<string> SendOtpAsync(ApplicationUser user, string subject);
        public Task<string> ConfirmEmailAsync(ApplicationUser user, string subject);
        public Task<string> SendEmail(string email, string Message, string? reason);

    }
}
