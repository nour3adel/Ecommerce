using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public decimal TotalPrice { get; set; }
        public string Status { get; set; }

        public string UserId { get; set; }

        public AppUser User { get; set; }  // virtual ??

        public IEnumerable<OrderItem> OrderItems { get; set; }




        

    }

    //public enum OrderStatus
    //{
    //    Pending,    // 0
    //    Shipped,    // 1
    //    Delivered   // 2
    //}
}
