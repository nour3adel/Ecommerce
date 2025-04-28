using E_CommerceAPI.ENTITES.DTOs.PaymentDTO;
using E_CommerceAPI.ENTITES.Models;
using Stripe.Checkout;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface ISessionService
    {

        //public Task<Session> CreateCheckoutSession();
        public Task<Session> CreateCheckoutSession(PaymentDto dto, ApplicationUser user);
    }
}
