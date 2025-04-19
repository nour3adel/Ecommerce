using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechXpress.Data.Contracts;
using TechXpress.Data.DataContext;



namespace TechXpress.Data
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<TechXpressDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("TechXpressConnString"));
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
