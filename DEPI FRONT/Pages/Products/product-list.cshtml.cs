using Ecommerce.Frontend.Helpers;
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Models.Product;
using Ecommerce.Frontend.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Frontend.Pages.Products
{
    public class product_listModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ILogger<product_listModel> _logger;

        public product_listModel(IProductService productService, ILogger<product_listModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        public Response<List<ProductDTO>> Products { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = HttpContext.Session.GetStringSafe("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogWarning("Access token not found in session. Redirecting to login page.");
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToPage("/Authentication/Login");
            }

            Products = await _productService.GetProductsAsync();

            return Page();
        }

    }
}
