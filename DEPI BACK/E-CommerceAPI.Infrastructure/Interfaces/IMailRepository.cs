using E_CommerceAPI.ENTITES.Models;

namespace E_CommerceAPI.Infrastructure.Interfaces
{
    public interface IMailRepository
    {
        Task SendResetPasswordEmailAsync(ApplicationUser user, string subject);

    }
}
