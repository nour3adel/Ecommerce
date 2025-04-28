using Ecommerce.Frontend.Helpers;
using Ecommerce.Frontend.Models.Carts;
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Services.Carts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Ecommerce.Frontend.Pages.Orders
{
    public class OrderCartModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly ILogger<OrderCartModel> _logger;

        // Constructor injection for CartService and Logger
        public OrderCartModel(ICartService cartService, ILogger<OrderCartModel> logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public Response<List<CartItemsDto>> CartItems { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Retrieve the access token from the session
                var accessToken = HttpContext.Session.GetStringSafe("AccessToken");

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    _logger.LogWarning("Access token not found in session. Redirecting to login page.");

                    return RedirectToPage("/Authentication/Login");
                }

                // Fetch all cart items using the CartService
                CartItems = await _cartService.GetAllItemsAsync();

                if (CartItems == null || !CartItems.Succeeded)
                {
                    _logger.LogWarning($"Failed to retrieve cart items. Status: {CartItems?.Message}");

                    return Page();
                }

            }
            catch (Exception ex)
            {
                // Log detailed error information
                _logger.LogError(ex, "An exception occurred while retrieving cart items.");

                // Provide user-friendly feedback


                // Optionally log stack trace for debugging
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return RedirectToPage("/Error");
            }

            // Return the page with the cart items
            return Page();
        }
        public async Task<IActionResult> OnPostDeleteCartItemAsync(int id)
        {
            try
            {
                var response = await _cartService.DeleteCartItem(id);
                if (!response.Succeeded)
                {
                    TempData["ErrorMessage"] = $"Failed to delete cart item. {response.Message}";
                    _logger.LogWarning("Failed to delete cart item ID {Id}: {Message}", id, response.Message);
                    return RedirectToPage(); // Reload current page
                }

                TempData["SuccessMessage"] = "Cart item removed successfully.";
                return RedirectToPage(); // Reload to reflect changes
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting cart item ID {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
                return RedirectToPage("/Error");
            }
        }
        public async Task<IActionResult> OnPostClearCartAsync()
        {
            try
            {
                var response = await _cartService.DeleteCartAsync();

                if (!response.Succeeded)
                {
                    TempData["ErrorMessage"] = $"Failed to clear cart. {response.Message}";
                    return RedirectToPage();
                }

                TempData["SuccessMessage"] = "Cart cleared successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart.");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
                return RedirectToPage("/Error");
            }
        }






    }
}