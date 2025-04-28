using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Infrastructure.Seeders
{
    public static class ProductSeeder
    {
        public static async Task Seed(ECommerceDbContext context)
        {
            // Check if any products exist
            if (!await context.Products.AnyAsync())
            {
                // Ensure categories and brands are seeded first
                var mouseCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Mouse");
                var appleBrand = await context.Brands.FirstOrDefaultAsync(b => b.Name == "Apple");

                var products = new List<Product>
            {
                new Product
                {
                    Name = "Apple Magic Mouse",
                    Description = "The latest Mouse with advanced features from Apple.",
                    Price = 999.99,
                    StockAmount = 100,
                    DiscountPercentage = 10,
                    Images = null,
                    CategoryId = mouseCategory?.Id,
                    BrandId = appleBrand?.Id
                },
                new Product
                {
                    Name = "MacBook Pro",
                    Description = "Powerful laptop for professionals.",
                    Price = 1999.99,
                    StockAmount = 50,
                    DiscountPercentage = 5,
                    Images = null,
                    CategoryId = mouseCategory?.Id,
                    BrandId = appleBrand?.Id
                }
            };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
