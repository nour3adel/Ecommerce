using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.ENTITES.DTOs.OrderDTO
{
    public class OrderDto
    {
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "opened"; // Default status

        public int? TrackNumberSeed { get; set; } // Optional track number seed

        public string? ShippingAddress { get; set; }
        public string? ShippingMethod { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? DeliverDate { get; set; }

        [Range(0, double.MaxValue)]
        public double? ShippingCost { get; set; } // Nullable shipping cost
    }
}











