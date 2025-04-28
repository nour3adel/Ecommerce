using E_CommerceAPI.ENTITES.DTOs.WishlistDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IWislistService
    {
        public Task<Response<WishlistDto>> GetWishlist(int id);
        public Task<Response<IEnumerable<WishlistDto>>> GetAllWishlists();

        public Task<Response<string>> AddWishlist(WishlistDto dto, ApplicationUser user);
        public Task<Response<string>> UpdateWishlist(int id, WishlistDto dto);
        public Task<Response<string>> DeleteWishlist(int id);
    }
}
