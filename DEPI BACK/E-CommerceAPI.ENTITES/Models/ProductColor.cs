using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceAPI.ENTITES.Models
{
    public class ProductColor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ColorName { get; set; } = null!;

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation Property
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }

}
