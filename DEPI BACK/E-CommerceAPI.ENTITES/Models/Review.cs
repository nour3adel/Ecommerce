using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceAPI.ENTITES.Models
{
    public class Review
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public int Rate { get; set; }

        [MaxLength(250)]
        public string? Comment { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public string CustomerId { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual ApplicationUser Customer { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
