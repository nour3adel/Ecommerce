namespace Ecommerce.Frontend.Models.Product
{
    public class FilterDto
    {
        public List<int>? CategoryId { get; set; }
        public List<int>? BrandId { get; set; }

        public int MinPrice { get; set; } = 0;
        public int MaxPrice { get; set; } = 5000;
        public double Rating { get; set; } = 0.0;
    }
}
