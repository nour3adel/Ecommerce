using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.OrderDTO;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "OrderItems")]

    public class OrderItemsController : AppControllerBase
    {
        private readonly IOrderItemService _orderItemService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<OrderItemsController> _logger;

        public OrderItemsController(IOrderItemService orderItemService, ILogger<OrderItemsController> logger, ICurrentUserService currentUserService)
        {
            _orderItemService = orderItemService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        [HttpGet("OrderItem/{id}")]
        public async Task<IActionResult> GetOrderItem(int id)
        {
            var currentUser = await _currentUserService.GetUserAsync();

            if (currentUser.Data == null)
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });

            var response = await _orderItemService.GetOrderItem(id);
            return NewResult(response);
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

            var response = await _orderItemService.GetAllItems(currentUser.Data);
            return NewResult(response);
        }

        //[HttpGet("ItemsInOrder/{orderId}")]
        //public async Task<IActionResult> GetItemsInOrder(int orderId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var items = await _orderItemService.GetItemsInOrder(orderId);
        //            return NewResult(items);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error occurred while fetching items in the order.");
        //            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
        //        }
        //    }
        //    return BadRequest(ModelState);
        //}

        [HttpPost("AddOrderItem")]
        public async Task<IActionResult> AddOrderItem([FromBody] AddOrderItemDto item)
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

            var response = await _orderItemService.AddOrderItem(item, currentUser.Data);
            return NewResult(response);
        }

        [HttpPut("EditOrderItem/{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, [FromBody] AddOrderItemDto item)
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

            var response = await _orderItemService.UpdateOrderItem(id, item, currentUser.Data);
            return NewResult(response);
        }

        [HttpDelete("DeleteOrderItem/{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var response = await _orderItemService.DeleteOrderItem(id);
            return NewResult(response);
        }

    }
}
