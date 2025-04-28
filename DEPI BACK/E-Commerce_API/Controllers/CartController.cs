using E_Commerce_API.Base;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Carts")]

    public class CartController : AppControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICurrentUserService _accountService;


        public CartController(ICartService cartService, ICurrentUserService accountService)
        {
            _cartService = cartService;
            _accountService = accountService;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
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

            var response = await _cartService.GetAllCarts();
            return NewResult(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(int id)
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

            var response = await _cartService.GetCart(id);
            return NewResult(response);
        }

        [Authorize]
        [HttpPost("AddCart")]
        public async Task<IActionResult> AddCart()
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

            var response = await _cartService.AddCart(currentUser.Data);
            return NewResult(response);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartById(int id)
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

            var response = await _cartService.DeleteCart(id);
            return NewResult(response);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteCart()
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

            var response = await _cartService.DeleteCurrentUserCart(currentUser.Data);
            return NewResult(response);
        }

    }
}
