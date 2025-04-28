using Ecommerce.Frontend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Frontend.Pages.Home;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string? Username { get; private set; }
    public string? Email { get; private set; }
    public string? Role { get; private set; }
    public string? UserId { get; private set; }
    public string? PhoneNumber { get; private set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
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
            // Use a helper method to extract all user info from token at once if possible
            var userInfo = JwtHelper.GetUserInfoFromToken(accessToken);
            Username = userInfo.Username;
            Email = userInfo.Email;
            Role = userInfo.Role;
            UserId = userInfo.UserId;
            PhoneNumber = userInfo.PhoneNumber;


            if (string.IsNullOrWhiteSpace(Username))
            {
                _logger.LogWarning("Token parsed but no username found. Possibly invalid or expired token.");
                TempData["ErrorMessage"] = "Invalid token. Please log in again.";
                return RedirectToPage("/Authentication/Login");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing JWT token. Redirecting to login.");
            TempData["ErrorMessage"] = "Authentication error. Please log in again.";
            return RedirectToPage("/Authentication/Login");
        }

        return Page();
    }
}
