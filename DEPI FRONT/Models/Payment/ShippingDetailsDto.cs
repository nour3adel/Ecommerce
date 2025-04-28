using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Frontend.Models.Payment
{
    public class ShippingDetailsDto
    {
        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string State { get; set; } = null!;

        [Required]
        public string ZipCode { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        public string Method { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public double Cost { get; set; }
    }
}
