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

        public DateTime UpdatedDate { get; set; }

        public string Status { get; set; }

        public string UserId { get; set; }

        public virtual AppUser User { get; set; }


        

    }   

    //public enum OrderStatus : byte
    //{
    //    pending =1,
    //    placed,

    //}
}
