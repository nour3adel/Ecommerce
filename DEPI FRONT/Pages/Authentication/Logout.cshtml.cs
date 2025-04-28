using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Frontend.Pages.Authentication
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            Console.WriteLine("Session cleared");
            // Redirect the user to the login page after logout
            return RedirectToPage("/Authentication/Login");
        }
    }
}
