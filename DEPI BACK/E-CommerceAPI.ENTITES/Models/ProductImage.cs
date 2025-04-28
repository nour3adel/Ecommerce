namespace E_CommerceAPI.ENTITES.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
