
using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace E_CommerceAPI.ENTITES.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Carts = new HashSet<Cart>();
            Orders = new HashSet<Order>();
            Wishlists = new HashSet<Wishlist>();
            Payments = new HashSet<Payment>();
            Reviews = new HashSet<Review>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        public string? Address { get; set; }
        [EncryptColumn]
        public string? Code { get; set; }

        public string? Image { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
