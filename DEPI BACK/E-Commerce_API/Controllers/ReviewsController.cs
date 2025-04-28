using System.Net;
using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.ReviewDTO;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reviews")]

    public class ReviewsController : AppControllerBase
    {
        private readonly IReviewService _reviewService;

        private readonly ICurrentUserService _accountService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ICurrentUserService accountService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet("Review/{id}")]
        public async Task<IActionResult> GetReview(int id)
        {
            var response = await _reviewService.GetReview(id);
            return NewResult(response);
        }

        [HttpGet("CustomerReviews")]
        public async Task<IActionResult> GetCustomerReviews()
        {
            try
            {
                var currentUser = await _accountService.GetUserAsync();

                if (currentUser.Data == null)
                {
                    return NewResult(new Response<string>
                    {
                        Succeeded = false,
                        StatusCode = HttpStatusCode.Unauthorized,
                        Message = "User is not authenticated.",
                    });
                }

                var response = await _reviewService.GetAllCustomerReviews(currentUser.Data);

                return response.Succeeded
                    ? NewResult(response)
                    : NotFound(response.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("User not authenticated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching customer reviews.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpGet("Product/{productId}/Reviews")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var response = await _reviewService.GetAllProductReviews(productId);
            return NewResult(response);
        }

        [HttpGet("CustomerReviewOnProduct/{prodId}")]
        public async Task<IActionResult> GetCustomerReviewOnProduct(int prodId)
        {
            var currentUser = await _accountService.GetUserAsync();

            if (currentUser.Data == null)
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });

            var response = await _reviewService.GetCustomerReviewOnProduct(currentUser.Data, prodId);
            return NewResult(response);
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] ReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = await _accountService.GetUserAsync();

            if (currentUser.Data == null)
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                }); 

            var response = await _reviewService.AddReview(reviewDto, currentUser.Data);
            return NewResult(response);
        }

        [HttpPut("UpdateReview/{id}")]
        public async Task<IActionResult> EditReview(int id, [FromBody] ReviewDto review)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _reviewService.UpdateReview(id, review);
            return NewResult(response);
        }

        [HttpDelete("DeleteReview/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var response = await _reviewService.DeleteReview(id);
            return NewResult(response);
        }

    }
}
