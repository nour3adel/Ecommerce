using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.ENTITES.DTOs.ReviewDTO
{
    public class ReviewDto
    {
        public int Rate { get; set; }
        [MaxLength(250)]
        public string Comment { get; set; } = null!;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string ProductName { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public int ProductId { get; set; }


    }
}

