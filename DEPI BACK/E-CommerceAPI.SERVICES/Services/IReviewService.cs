using E_CommerceAPI.ENTITES.DTOs.ReviewDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IReviewService
    {
        public Task<Response<ReviewDto>> GetReview(int id);
        public Task<Response<IEnumerable<ReviewDto>>> GetAllCustomerReviews(ApplicationUser user);
        public Task<Response<IEnumerable<ReviewDto>>> GetAllProductReviews(int productId);
        public Task<Response<IEnumerable<ReviewDto>>> GetCustomerReviewOnProduct(ApplicationUser user, int prodId);

        public Task<Response<string>> AddReview(ReviewDto review, ApplicationUser user);
        public Task<Response<string>> UpdateReview(int id, ReviewDto review);
        public Task<Response<string>> DeleteReview(int id);
    }
}
