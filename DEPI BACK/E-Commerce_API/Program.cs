using E_CommerceAPI.ENTITES.DTOs.StripeConfig;
using E_CommerceAPI.ENTITES.Helpers;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Seeders;
using E_CommerceAPI.SERVICES;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Stripe;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<IUrlHelper>(x =>
{
    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
    var factory = x.GetRequiredService<IUrlHelperFactory>();
    return factory.GetUrlHelper(actionContext);
});


#region Allow CORS
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{


    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
#endregion

#region SQL SERVER CONNECTION
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
#endregion

#region Dependency injections

builder.Services.AddInfrastructureDependencies()
                .AddServicesDependencies()
                .AddServiceRegisteration(builder.Configuration);

#endregion

// stripe config
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
var stripeConfig = builder.Configuration.GetSection("Stripe").Get<StripeConfig>();
StripeConfiguration.ApiKey = stripeConfig.SecretKey;
builder.Services.AddSingleton(stripeConfig);

//Email config
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


var app = builder.Build();

#region Seeder

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.Seed(roleManager);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await UserSeeder.Seed(userManager);

    var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    await BrandSeeder.Seed(context);
    await CategorySeeder.Seed(context);
    await ProductSeeder.Seed(context);
    await OrderSeeder.Seed(context);
}

#endregion

if (app.Environment.IsDevelopment())
{
    #region Swagger
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/All/swagger.json", "All");
        options.SwaggerEndpoint("/swagger/Accounts/swagger.json", "Accounts");
        options.SwaggerEndpoint("/swagger/Brands/swagger.json", "Brands");
        options.SwaggerEndpoint("/swagger/Carts/swagger.json", "Carts");
        options.SwaggerEndpoint("/swagger/CartItems/swagger.json", "CartItems");
        options.SwaggerEndpoint("/swagger/Categorys/swagger.json", "Categorys");
        options.SwaggerEndpoint("/swagger/Orders/swagger.json", "Orders");
        options.SwaggerEndpoint("/swagger/OrderItems/swagger.json", "OrderItems");
        options.SwaggerEndpoint("/swagger/Payments/swagger.json", "Payments");
        options.SwaggerEndpoint("/swagger/Products/swagger.json", "Products");
        options.SwaggerEndpoint("/swagger/Reviews/swagger.json", "Reviews");
        options.SwaggerEndpoint("/swagger/Wishlists/swagger.json", "Wishlists");
        options.SwaggerEndpoint("/swagger/WishlistItems/swagger.json", "WishlistItems");
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
        options.DefaultModelsExpandDepth(-1);
    });
    #endregion
}

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

