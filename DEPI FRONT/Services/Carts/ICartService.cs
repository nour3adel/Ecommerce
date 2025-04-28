using Ecommerce.Frontend.Models.Carts;
using Ecommerce.Frontend.Models.Common;

namespace Ecommerce.Frontend.Services.Carts
{
    public interface ICartService
    {
        Task<Response<List<CartDto>>> GetCartsAsync();
        Task<Response<List<CartItemsDto>>> GetAllItemsAsync();
        Task<Response<CartDto>> GetCartByIdAsync(int id);
        Task<Response<CartItemsDto>> GetCartItemByIdAsync(int id);
        Task<Response<string>> AddCartAsync();
        Task<Response<string>> AddCartItemAsync(AddCartItemDto addCartItemDto);
        Task<Response<string>> DeleteCartAsync();
        Task<Response<string>> DeleteCartItem(int Id);
        Task<Response<string>> UpdateCartItem(AddCartItemDto addCartItemDto);
    }
}
