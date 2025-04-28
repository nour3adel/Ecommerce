using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceAPI.ENTITES.Models
{
    public class Order
    {
        private static int _nextTrackNumber = 1000;

        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
            TrackNumber = GenerateTrackNumber(Id); // Call the static method to generate the track number
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } // Timestamp for order creation

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "opened"; // Default status


        public string TrackNumber { get; set; } // Auto-generated track number

        public string? ShippingAddress { get; set; }
        public string? ShippingMethod { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? DeliverDate { get; set; }

        [Range(0, double.MaxValue)]
        public double ShippingCost { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!;

        [ForeignKey("CustomerId")]
        public virtual ApplicationUser Customer { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        // Computed property for total amount (includes shipping cost)
        [NotMapped]
        public decimal TotalAmount => OrderItems.Sum(item => (decimal)item.Product.Price * item.Quantity) + (decimal)ShippingCost;

        // Audit fields
        public DateTime? UpdatedAt { get; set; } // Last updated timestamp

        /// <summary>
        /// Generates a unique and incrementing track number.
        /// </summary>
        /// <returns>The next available track number.</returns>
        public static string GenerateTrackNumber(int id)
        {
            return $"DEPI{1000 + id}";
        }

    }
}
