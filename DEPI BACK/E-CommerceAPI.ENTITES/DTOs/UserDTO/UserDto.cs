

using Microsoft.AspNetCore.Http;

namespace E_CommerceAPI.ENTITES.DTOs.UserDTO
{
    public class UserDto
    {
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public IFormFile? Image { get; set; }
    }
}
