using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

public class ResetPasswordModel : PageModel
{
    private readonly IAuthenticationService _authService;
    public bool isPostSubmitted = false;
    public ResetPasswordModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    public Response<string> ApiResponse { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        isPostSubmitted = true;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ApiResponse = await _authService.SendResetPasswordAsync(Email);
        if (ApiResponse.Succeeded)
        {
            TempData["ResetEmail"] = Email;
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
