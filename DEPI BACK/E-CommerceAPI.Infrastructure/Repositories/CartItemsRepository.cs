using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Infrastructure.Repositories
{
    internal class CartItemsRepository : GenericRepository<CartItem>, ICartItemsRepository
    {

        private readonly ECommerceDbContext _context;

        public CartItemsRepository(ECommerceDbContext context) : base(context)
        {

            _context = context;
        }


        public async Task<IEnumerable<CartItem>> GetItemsByCartID(int cartId)
        {
            var items = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CartId == cartId)
                .ToListAsync();
            return items;
        }
    }
}
