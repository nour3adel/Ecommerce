namespace Ecommerce.Frontend.Models.Category
{
    public class AddCategoryDto
    {
        public string Name { get; set; } = null!;
        public IFormFile? ImageFile { get; set; }
    }
}
