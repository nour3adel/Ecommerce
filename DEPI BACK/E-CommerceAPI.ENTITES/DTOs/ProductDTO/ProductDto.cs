namespace E_CommerceAPI.ENTITES.DTOs.ProductDTO
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public double Price { get; set; }
        public int StockAmount { get; set; }
        public List<ImagesDto>? Images { get; set; }
        public List<ColorsDto>? Colors { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public double? DiscountPercentage { get; set; }
        public double? AfterDiscount => DiscountPercentage.HasValue
            ? Math.Round(Price * (1 - DiscountPercentage.Value / 100), 2)
            : Math.Round(Price, 2);
    }
}
