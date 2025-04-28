using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.WishlistDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class WishlistService : ResponseHandler, IWislistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<string>> AddWishlist(WishlistDto dto, ApplicationUser user)
        {
            var list = _mapper.Map<Wishlist>(dto);
            list.CustomerId = user.Id;
            list.Customer = user;

            await _unitOfWork.Wishlists.AddAsync(list);
            await _unitOfWork.Save();
            return Success("Wishlist added successfully.");
        }

        public async Task<Response<string>> DeleteWishlist(int id)
        {
            var list = await _unitOfWork.Wishlists.GetByIdAsync(id);
            if (list == null)
            {
                return NotFound<string>("Wishlist not found.");
            }

            await _unitOfWork.Wishlists.DeleteAsync(list);
            await _unitOfWork.Save();
            return Deleted<string>("Wishlist deleted successfully.");
        }

        public async Task<Response<IEnumerable<WishlistDto>>> GetAllWishlists()
        {
            var lists = await _unitOfWork.Wishlists.Selectall();
            if (lists != null && lists.Count > 0)
            {
                var dto = _mapper.Map<IEnumerable<WishlistDto>>(lists);
                return Success(dto);
            }

            return NotFound<IEnumerable<WishlistDto>>("No wishlists found.");
        }

        public async Task<Response<WishlistDto>> GetWishlist(int id)
        {
            var wishlist = await _unitOfWork.Wishlists.GetByIdAsync(id);
            if (wishlist != null)
            {
                var dto = _mapper.Map<WishlistDto>(wishlist);
                return Success(dto);
            }
            return NotFound<WishlistDto>("Wishlist not found.");
        }

        public async Task<Response<string>> UpdateWishlist(int id, WishlistDto dto)
        {
            var list = await _unitOfWork.Wishlists.GetByIdAsync(id);
            if (list == null)
            {
                return NotFound<string>("Wishlist not found.");
            }

            var wish = _mapper.Map<Wishlist>(dto);
            wish.Id = id;
            wish.Customer = list.Customer;
            wish.CustomerId = list.CustomerId;
            wish.WishlistItems = list.WishlistItems;
            await _unitOfWork.Wishlists.UpdateAsync(list);
            await _unitOfWork.Save();
            return Updated<string>("Wishlist updated successfully.");
        }
    }

}
