using Ecommerce.Frontend.Validations;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Frontend.Models.Authentication
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50)]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [PasswordComplexity]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

    }
}
