using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.OrderDTO;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Orders")]
    public class OrderController : AppControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger, ICurrentUserService currentUserService)
        {
            _orderService = orderService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var currentUser = await _currentUserService.GetUserAsync();
            if (currentUser.Data == null)
            {
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });
            }

            var response = await _orderService.GetCustomerOrders();
            return NewResult(response);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var currentUser = await _currentUserService.GetUserAsync();

            if (currentUser.Data == null)
            {
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });
            }

            var response = await _orderService.GetOrderById(id);
            return NewResult(response);

        }

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder(OrderDto dto)
        {
            var currentUser = await _currentUserService.GetUserAsync();

            if (currentUser.Data == null)
            {
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });
            }

            var response = await _orderService.AddOrder(currentUser.Data, dto);
            return NewResult(response);

        }

        [HttpPut("EditOrder/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _orderService.UpdateOrder(id, dto);
                    return NewResult(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred while updating order with ID {id}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
                }
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteOrder()
        {
            var currentUser = await _currentUserService.GetUserAsync();

            if (currentUser.Data == null)
            {
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });
            }

            var response = await _orderService.DeleteCurrentUserOrder(currentUser.Data);
            return NewResult(response);
        }

        [Authorize]
        [HttpDelete("DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _orderService.DeleteOrder(id);
                    return NewResult(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred while deleting order with ID {id}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
                }
            }
            return BadRequest(ModelState);
        }


    }
}
