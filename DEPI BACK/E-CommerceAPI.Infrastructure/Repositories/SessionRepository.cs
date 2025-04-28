using E_CommerceAPI.Infrastructure.Interfaces;
using Stripe.Checkout;

namespace E_CommerceAPI.Infrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        public SessionRepository()
        {

        }

        public Task<Session> CreateCheckoutSession()
        {
            throw new NotImplementedException();
        }

    }
}
