using E_CommerceAPI.ENTITES.DTOs.CartDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface ICartService
    {

        Task<Response<CartDto>> GetCart(int id);
        Task<Response<IEnumerable<CartDto>>> GetAllCarts();
        Task<Response<string>> DeleteCurrentUserCart(ApplicationUser currentUser);
        Task<Response<string>> AddCart(ApplicationUser currentUser);
        //Task<Response<string>> UpdateCart(CartDto dto);
        Task<Response<string>> DeleteCart(int id);
    }
}
