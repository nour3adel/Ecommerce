using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceAPI.ENTITES.DTOs.OrderDTO
{
    public class OrderItemDto
    {
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Range(1, int.MaxValue)] // Quantity must be at least 1
        public int Quantity { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }

        [Precision(18, 2)]
        public decimal? Discount { get; set; } // Optional discount

        // Computed property for unit price
        [NotMapped]
        public decimal UnitPrice => Discount.HasValue ? (TotalPrice / Quantity) + Discount.Value : TotalPrice / Quantity;
    }
}
