namespace Ecommerce.Frontend.Models.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public int TotalStock { get; set; }
    }
}
