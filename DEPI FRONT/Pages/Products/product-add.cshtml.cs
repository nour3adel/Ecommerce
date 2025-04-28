using Ecommerce.Frontend.Helpers;
using Ecommerce.Frontend.Models.Brand;
using Ecommerce.Frontend.Models.Category;
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Models.Product;
using Ecommerce.Frontend.Services.Brands;
using Ecommerce.Frontend.Services.Categories;
using Ecommerce.Frontend.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Frontend.Pages.Products
{
    public class product_addModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly ILogger<product_addModel> _logger;

        public product_addModel(
                                IProductService productService,
                                ILogger<product_addModel> logger,
                                ICategoryService categoryService,
                                IBrandService brandService
                                )
        {
            _productService = productService;
            _logger = logger;
            _categoryService = categoryService;
            _brandService = brandService;
        }
        [BindProperty]
        public AddProductDto Product { get; set; } = new();
        public Response<List<CategoryDto>> Categories { get; set; }
        public Response<List<BrandDto>> Brands { get; set; }
        public Response<string> ApiResponse { get; set; } = new();
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
                Categories = await _categoryService.GetCategoriessAsync();
                Brands = await _brandService.GetBrandsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving data: {ex.Message}");
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

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDropdownsAsync();

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed. Product details are not valid.");
                return Page(); // Return the current page with validation errors
            }

            try
            {
                if (Product.ImageFiles == null || !Product.ImageFiles.Any())
                {
                    _logger.LogWarning("No images received in ImageFiles.");

                }
                else
                {
                    foreach (var file in Product.ImageFiles)
                    {
                        _logger.LogWarning($"Received file: {file.FileName}, Length: {file.Length}");
                    }
                }
                if (Product.ColorNames == null || !Product.ColorNames.Any())
                {
                    _logger.LogWarning("No colors received in ColorNames.");
                }
                else
                {
                    // Ensure color names are correctly split into a list if they are in a single comma-separated string
                    if (Product.ColorNames.Count == 1 && Product.ColorNames[0].Contains(","))
                    {
                        var colors = Product.ColorNames[0].Split(',').ToList();
                        Product.ColorNames = colors;
                    }
                    foreach (var color in Product.ColorNames)
                    {
                        _logger.LogInformation($"Received color: {color}");
                    }
                }


                ApiResponse = await _productService.AddProductAsync(Product);

                if (ApiResponse.Succeeded && !string.IsNullOrEmpty(ApiResponse.Data))
                {
                    _logger.LogInformation("Product added successfully.");
                    return RedirectToPage("/Products/product-list"); // Redirect to product list page
                }
                else
                {
                    _logger.LogWarning("Product addition failed. API response: {ApiResponse}", ApiResponse);

                    if (ApiResponse.Errors != null && ApiResponse.Errors.Any())
                    {
                        foreach (var error in ApiResponse.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while adding the product.");
                    }

                    return Page(); // Return the current page with API errors
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding the product.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while adding the product.");
                return Page(); // Return the current page with an error message
            }
        }

    }
}
