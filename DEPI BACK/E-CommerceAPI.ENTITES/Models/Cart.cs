using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceAPI.ENTITES.Models
{
    public class Cart
    {
        public Cart()
        {
            CartItems = new HashSet<CartItem>();
            IsClosed = false; // Default to open cart
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!;

        [ForeignKey("CustomerId")]
        public virtual ApplicationUser Customer { get; set; } = null!;

        public bool IsClosed { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }

        // Computed property for total cart value
        [NotMapped]
        public decimal TotalValue => CartItems.Sum(item => item.Subtotal);
    }
}
