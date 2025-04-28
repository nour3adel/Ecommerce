using Stripe.Checkout;

namespace E_CommerceAPI.Infrastructure.Interfaces
{
    public interface ISessionRepository
    {
        public Task<Session> CreateCheckoutSession();
    }
}
