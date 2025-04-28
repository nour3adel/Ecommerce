using Ecommerce.Frontend.Models.Authentication;
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Frontend.Pages.Authentication
{
    public class NewPasswordModel : PageModel
    {
        private readonly IAuthenticationService _authService;
        public bool IsPostSubmitted = false;

        public NewPasswordModel(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public ResetPasswordDto ResetPassword { get; set; } = new();

        public string Email { get; set; } = null!;

        public Response<string> ApiResponse { get; set; } = new();

        public IActionResult OnGet()
        {
            Email = TempData["ResetEmail"]?.ToString() ?? string.Empty;
            TempData.Keep("ResetEmail");

            if (string.IsNullOrWhiteSpace(Email))
            {
                return RedirectToPage("/Authentication/ResetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IsPostSubmitted = true;
            Email = TempData["ResetEmail"]?.ToString() ?? string.Empty;
            Console.WriteLine($"new: {ResetPassword.NewPassword}");
            Console.WriteLine($"c: {ResetPassword.ConfirmPassword}");
            ResetPassword.email = Email;

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Email))
            {
                Console.WriteLine($"asasasasasasas: {Email}");
                ModelState.AddModelError(string.Empty, "Invalid input.");
                return Page();
            }
            Console.WriteLine($"Email: {ResetPassword.email}");
            Console.WriteLine($"new: {ResetPassword.NewPassword}");
            Console.WriteLine($"c: {ResetPassword.ConfirmPassword}");

            ApiResponse = await _authService.ResetPasswordAsync(ResetPassword);
            Console.WriteLine($"message: {ApiResponse.Message}");

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
