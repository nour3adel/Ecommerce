namespace Ecommerce.Frontend.Models.Product
{
    public class AddProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public double Price { get; set; }
        public int StockAmount { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public double DiscountPercentage { get; set; }

        public List<IFormFile> ImageFiles { get; set; } = new();
        public List<string>? ColorNames { get; set; }
    }
}
