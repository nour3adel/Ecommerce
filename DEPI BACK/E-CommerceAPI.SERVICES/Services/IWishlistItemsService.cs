using E_CommerceAPI.ENTITES.DTOs.WishlistDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IWishlistItemsService
    {
        public Task<Response<WishlistItemsDto>> GetWishlistItem(int id);
        public Task<Response<IEnumerable<WishlistItemsDto>>> GetAllWishlistItems();
        public Task<Response<IEnumerable<WishlistItemsDto>>> GetItemsInWishlist(int listId);

        public Task<Response<string>> AddWishlistItem(WishlistItem item);
        public Task<Response<string>> UpdateWishlistItem(int id, WishlistItem item);
        public Task<Response<string>> DeleteWishlistItem(int id);
    }
}
