using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace E_CommerceAPI.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {

        private readonly ECommerceDbContext _context;

        public ReviewRepository(ECommerceDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Review>> GetAllCustomerReviews(ApplicationUser user)
        {
            var reviews = await _context.Reviews.Where(r => r.CustomerId == user.Id)
                .Include(r => r.Product)
                .ToListAsync();
            return reviews;

        }

        public async Task<IEnumerable<Review>> GetAllProductReviews(int productId)
        {
            var reviews = await _context.Reviews.Where(r => r.ProductId == productId)
                .Include(r => r.Product)
                .ToListAsync();
            return reviews;
        }

        public async Task<IEnumerable<Review>> GetCustomerReviewOnProduct(ApplicationUser user, int prodId)
        {

            var reviews = await _context.Reviews.Where(r => r.CustomerId == user.Id)
                .Include(r => r.Product).Where(p => p.ProductId == prodId)
                .ToListAsync();
            return reviews;
        }

    }
}
