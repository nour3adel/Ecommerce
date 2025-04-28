using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;

namespace E_CommerceAPI.Infrastructure.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        //Task<ResponseDto> CreateCheckoutSession(PaymentDto dto, ApplicationUser user);

    }
}
