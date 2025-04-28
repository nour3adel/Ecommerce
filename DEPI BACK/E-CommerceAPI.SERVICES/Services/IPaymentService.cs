using E_CommerceAPI.ENTITES.DTOs.PaymentDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IPaymentService
    {
        Task<Response<CheckoutResponse>> CreateCheckoutSession(PaymentDto dto, ApplicationUser user);

        public Task<Response<IEnumerable<PaymentDto>>> GetAllPayments();
        public Task<Response<PaymentDto>> GetPayment(int id);

        public Task<Response<string>> DeletePayment(int id);
    }
}
