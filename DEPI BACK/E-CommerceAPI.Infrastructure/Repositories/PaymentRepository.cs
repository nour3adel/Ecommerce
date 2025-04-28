using E_CommerceAPI.ENTITES.DTOs.PaymentDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace E_CommerceAPI.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {

        private readonly ECommerceDbContext _context;

        public PaymentRepository(ECommerceDbContext context, IOptions<StripeSettings> options) : base(context)
        {

            _context = context;
        }


    }
}
