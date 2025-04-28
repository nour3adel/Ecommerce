using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceAPI.ENTITES.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Range(1, int.MaxValue)] // Quantity must be at least 1
        public int Quantity { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; } // Price of the product at the time of purchase

        [Precision(18, 2)]
        public decimal? Discount { get; set; } // Optional discount

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        // Computed property for total price
        [NotMapped]
        public decimal TotalPrice => (UnitPrice * Quantity) - (Discount ?? 0);
    }
}
