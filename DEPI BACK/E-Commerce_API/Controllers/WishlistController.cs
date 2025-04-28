using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.WishlistDTO;
using E_CommerceAPI.SERVICES.Implementation;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Wishlists")]

    public class WishlistController : AppControllerBase
    {
        private readonly IWislistService _wishlistService;
        private readonly ICurrentUserService _accountService;

        public WishlistController(IWislistService wislistService, ICurrentUserService accountService)
        {
            _wishlistService = wislistService;
            _accountService = accountService;
        }

        [HttpGet("wishlist/{id}")]
        public async Task<IActionResult> GetWishlist(int id)
        {
            var response = await _wishlistService.GetWishlist(id);
            return NewResult(response);
        }

        [HttpGet("wishlists")]
        public async Task<IActionResult> GetAllWishlists()
        {
            var response = await _wishlistService.GetAllWishlists();
            return NewResult(response);
        }

        [HttpPost("wishlist")]
        public async Task<IActionResult> AddWishlist([FromBody] WishlistDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = await _accountService.GetUserAsync();

            if (currentUser.Data == null)
                return BadRequest("User is not authenticated.");

            var response = await _wishlistService.AddWishlist(dto, currentUser.Data);
            return NewResult(response);
        }

        [HttpPut("wishlist/{id}")]
        public async Task<IActionResult> EditWishlist(int id, [FromBody] WishlistDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _wishlistService.UpdateWishlist(id, dto);
            return NewResult(response);
        }

        [HttpDelete("wishlist/{id}")]
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            var response = await _wishlistService.DeleteWishlist(id);
            return NewResult(response);
        }

    }
}
