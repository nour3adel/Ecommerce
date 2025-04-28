using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Infrastructure.Seeders
{
    public static class RoleSeeder
    {
        public static async Task Seed(RoleManager<IdentityRole> rolemanager)
        {
            var roleCount = await rolemanager.Roles.CountAsync();
            if (roleCount <= 0)
            {
                await rolemanager.CreateAsync(new IdentityRole()
                {
                    Name = "Manager",
                    NormalizedName = "MANAGER"
                });
                await rolemanager.CreateAsync(new IdentityRole()
                {
                    Name = "Customer",
                    NormalizedName = "CUSTOMER"
                });
                await rolemanager.CreateAsync(new IdentityRole()
                {
                    Name = "Seller",
                    NormalizedName = "SELLER"
                });
            }
        }
    }
}
