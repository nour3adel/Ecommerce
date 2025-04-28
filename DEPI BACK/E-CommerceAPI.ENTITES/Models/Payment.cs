using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceAPI.ENTITES.Models
{

    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Method { get; set; } = null!;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = null!;

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!;

        [ForeignKey("CustomerId")]
        public virtual ApplicationUser Customer { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "pending";

    }

}
