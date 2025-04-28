using Ecommerce.Frontend.Helpers;
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Models.Product;
using Ecommerce.Frontend.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Frontend.Pages.Products
{
    public class product_detailsModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ILogger<product_detailsModel> _logger;

        public product_detailsModel(IProductService productService, ILogger<product_detailsModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public Response<ProductDTO> Product { get; set; } = new();



        public async Task<IActionResult> OnGetAsync()
        {

            var accessToken = HttpContext.Session.GetStringSafe("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogWarning("Access token not found in session. Redirecting to login page.");
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToPage("/Authentication/Login");
            }



            try
            {
                var response = await _productService.GetProductByIdAsync(Id);

                if (response.Succeeded && response.Data != null)
                {
                    Product = response;
                    return Page();
                }
                else
                {

                    _logger.LogWarning("Product with ID {ProductId} not found.", Id);
                    return NotFound(); // Return a 404 Not Found result
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 Internal Server Error
                _logger.LogError(ex, "An error occurred while fetching product details for ID {ProductId}.", Id);
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }


    }
}
