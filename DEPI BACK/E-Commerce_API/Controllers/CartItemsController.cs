using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.CartDTO;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "CartItems")]

    public class CartItemsController : AppControllerBase
    {
        private readonly ICartItemsService _cartItemsService;
        private readonly ICurrentUserService _currentUserService;

        public CartItemsController(ICartItemsService cartItemsService, ICurrentUserService cureentService)
        {
            _cartItemsService = cartItemsService;
            _currentUserService = cureentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var currentUser = await _currentUserService.GetUserAsync();

            if (currentUser.Data == null)
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });

            var response = await _cartItemsService.GetAllCartsItems(currentUser.Data);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var currentUser = await _currentUserService.GetUserAsync();

            if (currentUser.Data == null)
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });

            var response = await _cartItemsService.GetCartItem(id);
            return NewResult(response);
        }



        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddCartItemDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = await _currentUserService.GetUserAsync();

            if (currentUser.Data == null)
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });

            var response = await _cartItemsService.AddItemToCart(item, currentUser.Data);
            return NewResult(response);
        }

        [HttpPut("UpdateItem")]
        public async Task<IActionResult> EditCartItem([FromBody] AddCartItemDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = await _currentUserService.GetUserAsync();

            if (currentUser.Data == null)
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });

            var response = await _cartItemsService.UpdateCartItem(item, currentUser.Data);
            return NewResult(response);
        }

        [HttpDelete("DeleteItem/{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var response = await _cartItemsService.DeleteItemFromCart(id);
            return NewResult(response);
        }

        //[HttpGet("ItemsInCart/{cartId}")]
        //public async Task<IActionResult> GetItemsInCart(int cartId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var response = await _cartItemsService.GetItemsInCart(cartId);
        //        return NewResult(response);
        //    }
        //    return BadRequest(ModelState);
        //}

    }
}
