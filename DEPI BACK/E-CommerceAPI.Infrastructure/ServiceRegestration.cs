using E_CommerceAPI.ENTITES.Helpers;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace E_CommerceAPI.Infrastructure
{
    public static class ServiceRegestration
    {
        public static IServiceCollection AddServiceRegisteration(this IServiceCollection services, IConfiguration configuration)
        {

            #region Identity Configurations

            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                // Password settings.
                option.Password.RequireDigit = true;
                option.Password.RequireLowercase = true;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequireUppercase = true;
                option.Password.RequiredLength = 6;
                option.Password.RequiredUniqueChars = 1;

                //// Lockout settings.
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.AllowedForNewUsers = true;

                //// User settings.
                option.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                option.User.RequireUniqueEmail = true;
                option.SignIn.RequireConfirmedEmail = false;

            }).AddEntityFrameworkStores<ECommerceDbContext>().AddDefaultTokenProviders();

            #endregion

            //Email config
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);

            #region JWT Authentication

            var section = configuration.GetSection("jwtSettings");
            if (!section.Exists())
            {
                throw new Exception("jwtSettings section is missing in appsettings.json");
            }

            var jwtSettings = section.Get<JwtSettings>();
            services.AddSingleton(jwtSettings);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = jwtSettings.ValidateIssuer,
                   ValidIssuers = new[] { jwtSettings.Issuer },
                   ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                   ValidAudience = jwtSettings.Audience,
                   ValidateAudience = jwtSettings.ValidateAudience,
                   ValidateLifetime = jwtSettings.ValidateLifeTime,
                   ClockSkew = TimeSpan.Zero
               };
           });
            #endregion

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
                options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
                options.AddPolicy("Seller", policy => policy.RequireRole("Seller"));
            });

            #region Swagger Configurations

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("All", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ecommerce Managment System",
                    Description = "All Endpoints",

                });
                c.SwaggerDoc("Accounts", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Accounts",
                    Description = "Accounts Endpoint",

                });
                c.SwaggerDoc("Brands", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Brands",
                    Description = "Departments Endpoint",

                });
                c.SwaggerDoc("Carts", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Carts",
                    Description = "Carts Endpoint",

                });
                c.SwaggerDoc("CartItems", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CartItems",
                    Description = "CartItems Endpoint",

                });
                c.SwaggerDoc("Categorys", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Categorys",
                    Description = "Categorys Endpoint",

                });
                c.SwaggerDoc("Orders", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Orders",
                    Description = "Orders Endpoint",

                });
                c.SwaggerDoc("OrderItems", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "OrderItems",
                    Description = "OrderItems Endpoint",

                });
                c.SwaggerDoc("Payments", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Payments",
                    Description = "Payments Endpoint",

                });
                c.SwaggerDoc("Products", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Products",
                    Description = "Products Endpoint",

                });
                c.SwaggerDoc("Reviews", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Reviews",
                    Description = "Reviews Endpoint",

                });
                c.SwaggerDoc("Wishlists", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Wishlists",
                    Description = "Wishlists Endpoint",

                });
                c.SwaggerDoc("WishlistItems", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "WishlistItems",
                    Description = "WishlistItems Endpoint",

                });


                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var groupName = apiDesc.GroupName;
                    if (docName == "All")
                        return true;
                    return docName == groupName;
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }


                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                         {
                                            {
                                                new OpenApiSecurityScheme
                                                {
                                                    Reference = new OpenApiReference
                                                    {
                                                        Type = ReferenceType.SecurityScheme,
                                                        Id = JwtBearerDefaults.AuthenticationScheme
                                                    },Name="Bearer",
                            In=ParameterLocation.Header
                                                },
                                                Array.Empty<string>()
                                            }
                                        });

                c.EnableAnnotations();
            });

            #endregion

            return services;
        }
    }
}
