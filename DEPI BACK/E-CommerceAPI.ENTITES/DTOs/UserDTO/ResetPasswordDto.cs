using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.ENTITES.DTOs.UserDTO
{
    public class ResetPasswordDto
    {
        public string email { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;
        [Required]

        public string ConfirmPassword { get; set; } = null!;
    }
}
