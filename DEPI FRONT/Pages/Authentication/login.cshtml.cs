using Ecommerce.Frontend.Helpers;
using Ecommerce.Frontend.Models.Authentication;
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


public class LoginModel : PageModel
{
    private readonly IAuthenticationService _authService;

    public LoginModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public LoginDto Login { get; set; } = new();

    public Response<LoginResponse> ApiResponse { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ApiResponse = await _authService.LoginAsync(Login);

        if (ApiResponse.Succeeded && ApiResponse.Data != null)
        {
            // Save the access token in the session
            HttpContext.Session.SetStringSafe("AccessToken", ApiResponse.Data.AccessToken);

            // Redirect to the dashboard
            return RedirectToPage("/Home/Dashboard");
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
            ModelState.AddModelError(string.Empty, "An error occurred while logging in.");
        }

        return Page();
    }
}