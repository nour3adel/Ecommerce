using Ecommerce.Frontend.Helpers;
using Ecommerce.Frontend.Models.Carts;
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Models.Payment;
using Ecommerce.Frontend.Services.Carts;
using Ecommerce.Frontend.Services.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Ecommerce.Frontend.Pages.Orders
{
    public class OrderCheckoutModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderCheckoutModel> _logger;

        // Constructor injection for CartService and Logger
        public OrderCheckoutModel(ICartService cartService, IPaymentService paymentService, ILogger<OrderCheckoutModel> logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [BindProperty]
        public PaymentDto Payment { get; set; } = new();

        public Response<CheckoutResponse> Checkout { get; set; } = new();
        public Response<List<CartItemsDto>> CartItems { get; set; } = new();


        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = HttpContext.Session.GetStringSafe("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogWarning("Access token not found in session. Redirecting to login.");
                return RedirectToPage("/Authentication/Login");
            }

            try
            {

                CartItems = await _cartService.GetAllItemsAsync();

                if (CartItems == null || !CartItems.Succeeded)
                {
                    _logger.LogWarning("Failed to load cart items: {Message}", CartItems?.Message);
                    ModelState.AddModelError(string.Empty, "Failed to load cart items.");
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading cart items.");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return RedirectToPage("/Error");
            }
        }



        public async Task<IActionResult> OnPostAsync()
        {
            // Reload cart items to keep the page context correct.
            CartItems = await _cartService.GetAllItemsAsync();

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid payment form submission.");
                return Page();
            }

            try
            {
                Checkout = await _paymentService.AddPayment(Payment);

                if (Checkout.Succeeded && !string.IsNullOrWhiteSpace(Checkout.Data?.Url))
                {
                    _logger.LogInformation("Payment created successfully. Redirecting to payment URL.");
                    return Redirect(Checkout.Data.Url!); // Redirect to Stripe/other checkout URL
                }

                _logger.LogWarning("Payment failed: {Errors}", string.Join(", ", Checkout.Errors ?? new()));
                foreach (var error in Checkout.Errors ?? new List<string> { "Payment failed. Please try again." })
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during payment submission.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while processing your payment.");
                return Page();
            }
        }

    }
}
