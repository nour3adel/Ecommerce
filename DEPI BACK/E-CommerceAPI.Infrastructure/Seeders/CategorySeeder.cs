using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Infrastructure.Seeders
{
    public static class CategorySeeder
    {
        public static async Task Seed(ECommerceDbContext context)
        {
            // Check if any categories exist
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
            {
                new Category { Name = "Mouse", Image = "electronics.jpg" },
                new Category { Name = "Keyboard", Image = "clothing.jpg" },
                new Category { Name = "Headset", Image = "clothing.jpg" },
                new Category { Name = "Accessories", Image = "clothing.jpg" },
                new Category { Name = "Mobile Accessories", Image = "clothing.jpg" },
                new Category { Name = "Security", Image = "clothing.jpg" },
                new Category { Name = "Cables & Converters", Image = "clothing.jpg" },
                new Category { Name = "Speakers", Image = "clothing.jpg" },
                new Category { Name = "Network", Image = "clothing.jpg" },
                new Category { Name = "Cases & Monitors", Image = "clothing.jpg" },
                new Category { Name = "Playstation", Image = "books.jpg" }
            };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }
        }
    }
}
