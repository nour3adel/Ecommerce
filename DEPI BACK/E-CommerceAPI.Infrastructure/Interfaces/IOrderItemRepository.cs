using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;

namespace E_CommerceAPI.Infrastructure.Interfaces
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {

        public Task<ICollection<OrderItem>> GetItemsInOrder(int orderId);

    }
}
