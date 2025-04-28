using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;

namespace E_CommerceAPI.Infrastructure.Interfaces
{
    public interface ICartItemsRepository : IGenericRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetItemsByCartID(int cartId);
    }
}
