using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Infrastructure.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        private readonly ECommerceDbContext _context;

        public OrderItemRepository(ECommerceDbContext context) : base(context)
        {

            _context = context;
        }


        public async Task<ICollection<OrderItem>> GetItemsInOrder(int orderId)
        {
            var orderItems = await _context.OrderItems.Where(o => o.OrderId == orderId)
                .Include(o => o.Product).ToListAsync();
            return orderItems;

        }
    }


}
