using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Frontend.Models.Payment
{
    public class PaymentDto
    {
        [Required]
        [StringLength(3)] // ISO currency codes are 3 characters (e.g., USD, EUR)
        public string? CurrencyCode { get; set; }

        public string Description { get; set; } = "No description provided"; // Default value

        public string Method { get; set; } = "card"; // Default payment method

        public ShippingDetailsDto? ShippingDetails { get; set; }
    }
}
