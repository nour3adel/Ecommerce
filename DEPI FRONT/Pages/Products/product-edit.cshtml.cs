using Ecommerce.Frontend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Frontend.Pages.Products
{
    public class product_editModel : PageModel
    {
        private readonly ILogger<product_editModel> _logger;
        public product_editModel(ILogger<product_editModel> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = HttpContext.Session.GetStringSafe("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogWarning("Access token not found in session. Redirecting to login page.");
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToPage("/Authentication/Login");
            }



            return Page();
        }
    }
}
