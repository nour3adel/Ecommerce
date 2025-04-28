using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "WishlistItems")]

    public class WishlistItemsController : AppControllerBase
    {
        private readonly IWishlistItemsService _wishlistItemsService;

        public WishlistItemsController(IWishlistItemsService wishlistItemsService)
        {
            _wishlistItemsService = wishlistItemsService;
        }

        [HttpGet("wishlist-item/{id}")]
        public async Task<IActionResult> GetWishlistItem(int id)
        {
            var response = await _wishlistItemsService.GetWishlistItem(id);
            return NewResult(response);
        }

        [HttpGet("wishlist-items")]
        public async Task<IActionResult> GetAllWishlistItems()
        {
            var response = await _wishlistItemsService.GetAllWishlistItems();
            return NewResult(response);
        }

        [HttpGet("items-in-list/{listId}")]
        public async Task<IActionResult> GetItemsInList(int listId)
        {
            var response = await _wishlistItemsService.GetItemsInWishlist(listId);
            return NewResult(response);
        }

        [HttpPost("wishlist-item")]
        public async Task<IActionResult> AddItemToList([FromBody] WishlistItem item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _wishlistItemsService.AddWishlistItem(item);
            return NewResult(response);
        }

        [HttpPut("wishlist-item/{id}")]
        public async Task<IActionResult> EditWishlistItem(int id, [FromBody] WishlistItem item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _wishlistItemsService.UpdateWishlistItem(id, item);
            return NewResult(response);
        }

        [HttpDelete("wishlist-item/{id}")]
        public async Task<IActionResult> DeleteWishlistItem(int id)
        {
            var response = await _wishlistItemsService.DeleteWishlistItem(id);
            return NewResult(response);
        }

    }
}
