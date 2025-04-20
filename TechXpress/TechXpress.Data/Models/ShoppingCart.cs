using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress.Data.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public AppUser User { get; set; }

        public IEnumerable<CartItem> CartItems { get; set; }
    }
}
