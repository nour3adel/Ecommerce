using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Frontend.Models.Authentication
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public  string email { get; set; } 

        [Required(ErrorMessage = "Password is required.")]
        public  string password { get; set; } 
    }
}
