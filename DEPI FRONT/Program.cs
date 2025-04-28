using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Services.Authentication;
using Ecommerce.Frontend.Services.Brands;
using Ecommerce.Frontend.Services.Carts;
using Ecommerce.Frontend.Services.Categories;
using Ecommerce.Frontend.Services.Payment;
using Ecommerce.Frontend.Services.Products;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

//===========================
// Configure Services
//===========================

// Add Session
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// App Settings
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Add HttpClients
ConfigureHttpClients(builder);

// Add Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Add Razor Pages
builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.EnableEndpointRouting = true;
    });

var app = builder.Build();

//===========================
// Configure Middleware
//===========================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/F404"); // <-- Show custom 404 page
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseStatusCodePagesWithReExecute("/F404"); // <-- Even in dev if you prefer
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();         // Enable session
app.UseAuthorization();   // Auth

// Map Razor Pages
app.MapRazorPages();

// Redirect root to Dashboard
app.MapGet("/", context =>
{
    context.Response.Redirect("/Home/Dashboard");
    return Task.CompletedTask;
});

app.Run();

//===========================
// Local Helper Methods
//===========================

void ConfigureHttpClients(WebApplicationBuilder builder)
{
    string? apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    if (string.IsNullOrWhiteSpace(apiBaseUrl))
    {
        throw new InvalidOperationException("ApiSettings:BaseUrl is not configured in appsettings.json.");
    }

    builder.Services.AddHttpClient<IProductService, ProductService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });
    builder.Services.AddHttpClient<ICategoryService, CategoryService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });
    builder.Services.AddHttpClient<IBrandService, BrandService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });
    builder.Services.AddHttpClient<ICartService, CartService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });
    builder.Services.AddHttpClient<IPaymentService, PaymentService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });
}
