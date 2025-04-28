using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.PaymentDTO;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Payments")]

    public class PaymentController : AppControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ICurrentUserService _accountService;

        public PaymentController(IPaymentService paymentService, ICurrentUserService accountService)
        {
            _accountService = accountService;
            _paymentService = paymentService;

        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> CreateCheckOutSession(PaymentDto dto)
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

            if (ModelState.IsValid)
            {
                var response = await _paymentService.CreateCheckoutSession(dto, currentUser.Data);
                return NewResult(response);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("Payment/{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            if (ModelState.IsValid)
            {
                var response = await _paymentService.GetPayment(id);
                return NewResult(response);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("AllPayments")]
        public async Task<IActionResult> GetAllPayments()
        {
            if (ModelState.IsValid)
            {
                var response = await _paymentService.GetAllPayments();
                return NewResult(response);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeletePayment/{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            if (ModelState.IsValid)
            {
                var response = await _paymentService.DeletePayment(id);
                return NewResult(response);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            return Ok("Succeeded");
        }

        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            return Ok("Canceled");
        }



    }
}
