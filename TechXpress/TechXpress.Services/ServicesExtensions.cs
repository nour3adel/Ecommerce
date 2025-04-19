using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TechXpress.Services.Contracts;
using TechXpress.Services.Services;

namespace TechXpress.Services
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServicesLayer(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            return services;
        }
    }
}
