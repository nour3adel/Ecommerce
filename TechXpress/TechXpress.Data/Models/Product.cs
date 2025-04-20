using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress.Data.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; }

        public string Color { get; set; }
        public string StorageCapacity { get; set; }
        public string ScreenSize { get; set; }

        public decimal Weight {  get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
