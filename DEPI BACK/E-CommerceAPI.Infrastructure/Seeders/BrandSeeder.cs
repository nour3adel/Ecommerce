using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Infrastructure.Seeders
{
    public static class BrandSeeder
    {
        public static async Task Seed(ECommerceDbContext context)
        {
            // Check if any brands exist
            if (!await context.Brands.AnyAsync())
            {
                var brands = new List<Brand>
            {
                new Brand { Name = "Apple", Email = "support@apple.com", Phone = "+1-800-APPLE" },
                new Brand { Name = "Samsung", Email = "support@samsung.com", Phone = "+1-800-SAMSUNG" },
                new Brand { Name = "Sony", Email = "support@sony.com", Phone = "+1-800-SONY" }
            };

                context.Brands.AddRange(brands);
                await context.SaveChangesAsync();
            }
        }
    }
}
