﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechXpress.Data.Contracts;
using TechXpress.Data.DataContext;
using TechXpress.Data.Models;



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

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;

            })
                .AddEntityFrameworkStores<TechXpressDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
