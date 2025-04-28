using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceAPI.ENTITES.Models
{
    public class Product
    {
        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
            WishlistItems = new HashSet<WishlistItem>();
            CartItems = new HashSet<CartItem>();
            Reviews = new HashSet<Review>();
            Images = new HashSet<ProductImage>();
            Colors = new HashSet<ProductColor>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public double Price { get; set; }
        public int StockAmount { get; set; }
        public double? DiscountPercentage { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public double? AfterDiscount => DiscountPercentage.HasValue
            ? Math.Round(Price * (1 - DiscountPercentage.Value / 100), 2)
            : Math.Round(Price, 2);

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand? Brand { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<WishlistItem> WishlistItems { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<ProductImage>? Images { get; set; }
        public virtual ICollection<ProductColor> Colors { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

    }
}
