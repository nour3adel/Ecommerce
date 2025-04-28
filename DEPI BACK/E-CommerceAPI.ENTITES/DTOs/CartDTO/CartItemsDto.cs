namespace E_CommerceAPI.ENTITES.DTOs.CartDTO
{

    public class CartItemsDto
    {
        public int Id { get; set; }

        public int Quantity { get; set; }
        public string? ProductName { get; set; }
        public int ProductId { get; set; }
        public double ProductPrice { get; set; }
        public string? ProductImage { get; set; }
        public string? ProductCategory { get; set; }
        public string? ProductBrand { get; set; }
    }

}
