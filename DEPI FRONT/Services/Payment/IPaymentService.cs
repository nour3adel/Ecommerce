
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Models.Payment;

namespace Ecommerce.Frontend.Services.Payment
{
    public interface IPaymentService
    {
        Task<Response<List<PaymentDto>>> GetAllPayments();
        Task<Response<CheckoutResponse>> AddPayment(PaymentDto paymentDto);
        Task<Response<PaymentDto>> GetPayment(int id);
        Task<Response<string>> DeletePayment();
        Task<Response<string>> DeletePayment(int Id);
    }
}
