using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.ENTITES.DTOs.PaymentDTO
{
    public class PaymentDto
    {
        [Required]
        [StringLength(3)] // ISO currency codes are 3 characters (e.g., USD, EUR)
        public string? CurrencyCode { get; set; }

        public string Description { get; set; } = "No description provided"; // Default value

        public string Method { get; set; } = "card"; // Default payment method

        //[Required]
        //[Precision(18, 2)]
        //public decimal Amount { get; set; }

        public ShippingDetailsDto? ShippingDetails { get; set; }
    }
}
