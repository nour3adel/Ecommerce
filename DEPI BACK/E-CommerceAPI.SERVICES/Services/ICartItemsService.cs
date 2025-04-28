using E_CommerceAPI.ENTITES.DTOs.CartDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface ICartItemsService
    {
        Task<Response<IEnumerable<CartItemsDto>>> GetAllCartsItems(ApplicationUser applicationUser);
        Task<Response<CartItemsDto>> GetCartItem(int id);
        //Task<Response<List<CartItemsDto>>> GetItemsInCart(int cartId);
        Task<Response<string>> AddItemToCart(AddCartItemDto item, ApplicationUser applicationUser);
        Task<Response<string>> UpdateCartItem(AddCartItemDto itemDto, ApplicationUser currentUser);
        Task<Response<string>> DeleteItemFromCart(int id);
    }
}
