using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.ReviewDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;


namespace E_CommerceAPI.SERVICES.Implementation
{
    public class ReviewService : ResponseHandler, IReviewService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<string>> AddReview(ReviewDto reviewDto, ApplicationUser user)
        {
            // Validate if the product exists
            var product = await _unitOfWork.Products.GetByIdAsync(reviewDto.ProductId);
            if (product == null)
            {
                return NotFound<string>("Product not found.");
            }

            // Create a new Review entity from the ReviewDto
            var review = new Review
            {
                Rate = reviewDto.Rate,
                Comment = reviewDto.Comment,
                Date = DateTime.UtcNow,
                CustomerId = user.Id,
                ProductId = reviewDto.ProductId
            };

            // Add the review to the database
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.Save();

            return Success("Review added successfully.");
        }

        public async Task<Response<string>> DeleteReview(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound<string>("Review not found.");

            }
            await _unitOfWork.Reviews.UpdateAsync(review);
            await _unitOfWork.Save();
            return Deleted<string>("Review deleted successfully.");
        }

        public async Task<Response<IEnumerable<ReviewDto>>> GetAllCustomerReviews(ApplicationUser user)
        {
            if (user == null)
            {
                return BadRequest<IEnumerable<ReviewDto>>("User not found.");
            }

            var reviews = await _unitOfWork.Reviews.GetAllCustomerReviews(user);

            if (reviews != null && reviews.Any())
            {
                var dto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
                return Success(dto);
            }

            return NotFound<IEnumerable<ReviewDto>>("No reviews found for this user.");
        }

        public async Task<Response<IEnumerable<ReviewDto>>> GetAllProductReviews(int productId)
        {
            var Products = await _unitOfWork.Products.GetByIdAsync(productId);
            if (Products == null)
            {
                return NotFound<IEnumerable<ReviewDto>>("This product not found.");

            }
            var reviews = await _unitOfWork.Reviews.GetAllProductReviews(productId);
            if (reviews != null && reviews.Count() > 0)
            {
                var dto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
                return Success(dto);
            }
            return NotFound<IEnumerable<ReviewDto>>("There is no reviews for this product.");

        }

        public async Task<Response<IEnumerable<ReviewDto>>> GetCustomerReviewOnProduct(ApplicationUser user, int prodId)
        {
            var Products = await _unitOfWork.Products.GetByIdAsync(prodId);
            if (Products == null)
            {
                return NotFound<IEnumerable<ReviewDto>>("This product not found.");

            }


            var reviews = await _unitOfWork.Reviews.GetCustomerReviewOnProduct(user, prodId);

            if (reviews != null && reviews.Count() > 0)
            {
                var dto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
                return Success(dto);
            }
            return BadRequest<IEnumerable<ReviewDto>>("There is no reviews by this user for the product.");
        }

        public async Task<Response<ReviewDto>> GetReview(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review != null)
            {
                var dto = _mapper.Map<ReviewDto>(review);
                return Success(dto);
            }
            return NotFound<ReviewDto>("Review not found.");
        }

        public async Task<Response<string>> UpdateReview(int id, ReviewDto dto)
        {
            var reviewCheck = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (reviewCheck == null)
            {
                return BadRequest<string>("Review not found.");
            }
            var newReview = _mapper.Map<ENTITES.Models.Review>(dto);
            newReview.Id = id;
            newReview.Date = DateTime.UtcNow;
            newReview.Comment = dto.Comment;
            newReview.Rate = dto.Rate;

            await _unitOfWork.Reviews.UpdateAsync(newReview);
            await _unitOfWork.Save();

            return Updated<string>("Review Updated Successfully");
        }
    }
}
