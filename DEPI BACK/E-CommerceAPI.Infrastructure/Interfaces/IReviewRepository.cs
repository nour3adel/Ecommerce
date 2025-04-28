using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;

namespace E_CommerceAPI.Infrastructure.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {

        public Task<IEnumerable<Review>> GetAllCustomerReviews(ApplicationUser user);
        public Task<IEnumerable<Review>> GetAllProductReviews(int productId);
        public Task<IEnumerable<Review>> GetCustomerReviewOnProduct(ApplicationUser user, int prodId);


    }
}
