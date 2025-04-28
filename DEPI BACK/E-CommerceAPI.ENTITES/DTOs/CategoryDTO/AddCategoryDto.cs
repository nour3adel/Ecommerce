using Microsoft.AspNetCore.Http;

namespace E_CommerceAPI.ENTITES.DTOs.CategoryDTO
{
    public class AddCategoryDto
    {
        public string Name { get; set; } = null!;
        public IFormFile? ImageFile { get; set; }

    }
}
