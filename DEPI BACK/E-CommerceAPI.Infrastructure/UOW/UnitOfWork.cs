using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.PaymentDTO;
using E_CommerceAPI.ENTITES.Helpers;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Interfaces;
using E_CommerceAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace E_CommerceAPI.Infrastructure.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<EmailSettings> _settings;
        IOptions<StripeSettings> _stripeSettings;

        public IGenericRepository<ApplicationUser> Customers { get; private set; }
        public IProductRepository Products { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IOrderItemRepository OrderItems { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IGenericRepository<Cart> Carts { get; private set; }
        public ICartItemsRepository CartItems { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IGenericRepository<Wishlist> Wishlists { get; private set; }
        public IGenericRepository<WishlistItem> WishlistItems { get; private set; }
        public ISessionRepository Sessions { get; private set; }
        public IMailRepository Mails { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public IBrandRepository Brands { get; private set; }
        public IGenericRepository<ProductColor> ProductColors { get; private set; }
        public IGenericRepository<ProductImage> ProductImages { get; private set; }
        public UnitOfWork(ECommerceDbContext context, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IOptions<EmailSettings> setting, IOptions<StripeSettings> stripeSettings)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _settings = setting;
            _stripeSettings = stripeSettings;

            Customers = new GenericRepository<ApplicationUser>(_context);
            Carts = new GenericRepository<Cart>(_context);
            CartItems = new CartItemsRepository(_context);
            Categories = new CategoryRepository(_context);
            Orders = new GenericRepository<Order>(_context);
            OrderItems = new OrderItemRepository(_context);
            Products = new ProductRepository(_context);
            Reviews = new ReviewRepository(_context);
            Wishlists = new GenericRepository<Wishlist>(_context);
            WishlistItems = new GenericRepository<WishlistItem>(_context);
            Sessions = new SessionRepository();
            Mails = new MailRepository(_context, setting);
            Payments = new PaymentRepository(_context, stripeSettings);
            Brands = new BrandRepository(_context);
            ProductColors = new GenericRepository<ProductColor>(_context);
            ProductImages = new GenericRepository<ProductImage>(_context);
        }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
