using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Interfaces;


namespace E_CommerceAPI.Infrastructure.UOW
{
    public interface IUnitOfWork : IDisposable
    {

        IGenericRepository<ApplicationUser> Customers { get; }
        IProductRepository Products { get; }
        IGenericRepository<Order> Orders { get; }
        IOrderItemRepository OrderItems { get; }
        ICategoryRepository Categories { get; }
        IGenericRepository<Cart> Carts { get; }
        IGenericRepository<ProductColor> ProductColors { get; }
        IGenericRepository<ProductImage> ProductImages { get; }
        ICartItemsRepository CartItems { get; }
        IReviewRepository Reviews { get; }
        IGenericRepository<Wishlist> Wishlists { get; }
        IGenericRepository<WishlistItem> WishlistItems { get; }
        ISessionRepository Sessions { get; }
        IMailRepository Mails { get; }
        IPaymentRepository Payments { get; }
        IBrandRepository Brands { get; }

        Task<int> Save();
    }
}
