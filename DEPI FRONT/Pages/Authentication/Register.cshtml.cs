using Ecommerce.Frontend.Models.Authentication;
using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


public class RegisterModel : PageModel
{
    private readonly IAuthenticationService _authService;
    public bool IsPostSubmitted { get; set; } = false;

    public RegisterModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public RegisterDto register { get; set; } = new();

    public Response<string> ApiResponse { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        IsPostSubmitted = true;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ApiResponse = await _authService.RegisterAsync(register);
        Console.WriteLine(ApiResponse.Message);

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
            ModelState.AddModelError(string.Empty, "An error occurred while logging in.");
        }

        return Page();
    }
}