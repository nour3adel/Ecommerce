using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Infrastructure.Seeders
{
    public static class OrderSeeder
    {
        public static async Task Seed(ECommerceDbContext context)
        {
            // Check if any orders exist
            if (!await context.Orders.AnyAsync())
            {
                var customer = await context.Users.FirstOrDefaultAsync(u => u.Email == "nour3del145@gmail.com");

                if (customer == null)
                {
                    Console.WriteLine("No customer found. Please seed users first.");
                    return;
                }
                var order = new Order
                {

                    CreatedAt = DateTime.UtcNow,
                    TrackNumber = "DEPI1009",
                    DeliverDate = DateTime.UtcNow.AddDays(5),
                    ShippingCost = 10.0,
                    ShippingMethod = "Express",
                    ShippingDate = DateTime.UtcNow.AddDays(1),
                    Status = "closed",
                    ShippingAddress = "456 Cairo egypt",
                    CustomerId = customer.Id
                };

                await context.Orders.AddAsync(order);
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Error occurred while seeding orders: {ex.InnerException?.Message}");
                }
            }
        }
    }
}
