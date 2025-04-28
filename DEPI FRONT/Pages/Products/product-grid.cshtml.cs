using Ecommerce.Frontend.Helpers;
using Ecommerce.Frontend.Models.Brand;
using Ecommerce.Frontend.Models.Carts;
using Ecommerce.Frontend.Models.Category;
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Models.Product;
using Ecommerce.Frontend.Services.Brands;
using Ecommerce.Frontend.Services.Carts;
using Ecommerce.Frontend.Services.Categories;
using Ecommerce.Frontend.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Frontend.Pages.Products
{
    public class product_gridModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly ILogger<product_gridModel> _logger;
        public product_gridModel(IProductService productService,
                                 ICategoryService categoryService,
                                 IBrandService brandService,
                                 ICartService cartService,
                                 ILogger<product_gridModel> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _cartService = cartService;
            _logger = logger;

        }

        public Response<List<ProductDTO>> Products { get; set; } = new();
        public Response<List<CategoryDto>> Categories { get; set; }
        public Response<List<BrandDto>> Brands { get; set; }
        public Response<string> ApiResponse { get; set; } = new();
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadDropdownsAsync();
                Products = await _productService.GetProductsAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving data: {ErrorMessage}", ex.Message);
                TempData["ErrorMessage"] = "An error occurred while fetching data. Please try again later.";
                return RedirectToPage("/Error");
            }
            return Page();
        }
        private async Task LoadDropdownsAsync()
        {
            Categories = await _categoryService.GetCategoriessAsync();
            Brands = await _brandService.GetBrandsAsync();
        }
        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity = 1)
        {
            try
            {
                var accessToken = HttpContext.Session.GetStringSafe("AccessToken");

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    _logger.LogWarning("Access token not found in session. Redirecting to login page.");
                    TempData["ErrorMessage"] = "Session expired. Please log in again.";
                    return RedirectToPage("/Authentication/Login");
                }

                // Call the CartService to add the product to the cart
                AddCartItemDto addCartItemDto = new()
                {
                    Productid = productId,
                    Quantity = quantity
                };
                await _cartService.AddCartAsync();

                var result = await _cartService.AddCartItemAsync(addCartItemDto);

                if (!result.Succeeded)
                {
                    _logger.LogWarning($"Failed to add product to cart. Status: {result.Message}");
                    return RedirectToPage("/Orders/order-cart");
                }

                // Redirect to the cart page after successful addition
                return RedirectToPage("/Orders/order-cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while adding product to cart.");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToPage("/Error");
            }
        }



    }
}
