using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.ENTITES.DTOs.OrderDTO
{
    public class AddOrderItemDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; } // Renamed from productId

        [Range(1, int.MaxValue)] // Quantity must be at least 1
        public int Quantity { get; set; }
    }
}
