using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Frontend.Pages.Authentication
{
    public class OTPModel : PageModel
    {
        private readonly IAuthenticationService _authService;
        public bool IsPostSubmitted = false;

        public OTPModel(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Code { get; set; } = null!;

        public string Email { get; set; } = null!;

        public Response<string> ApiResponse { get; set; } = new();

        public void OnGet()
        {
            Email = TempData["ResetEmail"]?.ToString() ?? string.Empty;
            TempData.Keep("ResetEmail"); // keep value for next request too

            if (string.IsNullOrWhiteSpace(Email))
            {
                // Redirect to reset password if email is not available
                RedirectToPage("/Authentication/ResetPassword");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IsPostSubmitted = true;
            Email = TempData["ResetEmail"]?.ToString() ?? string.Empty;
            TempData.Keep("ResetEmail"); // keep value again

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError(string.Empty, "Invalid input.");
                return Page();
            }

            ApiResponse = await _authService.ConfirmResetPassword(Code, Email);

            if (ApiResponse.Succeeded && ApiResponse.Data != null)
            {
                return Page();
            }

            if (ApiResponse.Errors != null && ApiResponse.Errors.Any())
            {
                foreach (var error in ApiResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
            }

            return Page();
        }
    }
}
