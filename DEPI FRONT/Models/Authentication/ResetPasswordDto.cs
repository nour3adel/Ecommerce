using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Frontend.Models.Authentication
{
    public class ResetPasswordDto
    {
        public string email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

}
