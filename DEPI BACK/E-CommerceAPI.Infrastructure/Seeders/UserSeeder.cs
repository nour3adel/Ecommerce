using E_CommerceAPI.ENTITES.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Infrastructure.Seeders
{
    public static class UserSeeder
    {
        public static async Task Seed(UserManager<ApplicationUser> usermanager)
        {
            var roleCount = await usermanager.Users.CountAsync();
            if (roleCount <= 0)
            {
                var admin = new ApplicationUser()
                {
                    UserName = "admin",
                    Email = "admin@project.com",
                    FirstName = "admin",
                    LastName = "admin",
                    PhoneNumber = "123456",
                    Address = "Egypt",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };


                var result = await usermanager.CreateAsync(admin, "1234Admin#");
                if (result.Succeeded)
                {
                    await usermanager.AddToRoleAsync(admin, "Manager");
                }


                var customer = new ApplicationUser()
                {
                    UserName = "nour3del",
                    Email = "nour3del145@gmail.com",
                    FirstName = "Nour",
                    LastName = "Adel",
                    PhoneNumber = "01006377497",
                    Address = "Obour City",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                await usermanager.CreateAsync(customer, "1234Nour##");
                await usermanager.AddToRoleAsync(customer, "Customer");

                var defaultuser3 = new ApplicationUser()
                {
                    UserName = "AHmed",
                    Email = "ahmed@gmail.com",
                    FirstName = "ahmed",
                    LastName = "adel",
                    PhoneNumber = "123456",
                    Address = "Luxor",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                await usermanager.CreateAsync(defaultuser3, "1234Ahmed#");
                await usermanager.AddToRoleAsync(defaultuser3, "Seller");
            }
        }
    }
}
