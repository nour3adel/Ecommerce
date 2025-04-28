using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.WishlistDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class WishlistItemsService : ResponseHandler, IWishlistItemsService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public WishlistItemsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<string>> AddWishlistItem(WishlistItem item)
        {
            var existingItem = await _unitOfWork.Wishlists.GetByIdAsync(item.WishlistId);
            if (existingItem == null)
            {
                return BadRequest<string>("Wish list you try to add item to is not found.");

            }
            var existProduct = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (existProduct == null)
            {
                return BadRequest<string>("Product you try to add is not found.");
            }

            item.Wishlist = existingItem;
            item.Product = existProduct;
            await _unitOfWork.WishlistItems.AddAsync(item);
            await _unitOfWork.Save();
            return Success("Item added successfully.");
        }

        public async Task<Response<string>> DeleteWishlistItem(int id)
        {
            var item = await _unitOfWork.WishlistItems.GetTableNoTracking().Where(i => i.Id == id)
               .Include(i => i.Wishlist).Include(i => i.Product).FirstOrDefaultAsync();
            if (item == null)
            {
                return NotFound<string>("Item not found.");
            }

            await _unitOfWork.WishlistItems.DeleteAsync(item);
            await _unitOfWork.Save();
            return Deleted<string>("Item deleted successfully.");
        }

        public async Task<Response<IEnumerable<WishlistItemsDto>>> GetAllWishlistItems()
        {
            var items = await _unitOfWork.WishlistItems.GetTableNoTracking()
               .Include(i => i.Wishlist).Include(i => i.Product)
               .ToListAsync();

            if (items != null && items.Count > 0)
            {
                var dto = _mapper.Map<IEnumerable<WishlistItemsDto>>(items);
                return Success(dto);
            }
            return NotFound<IEnumerable<WishlistItemsDto>>("There is no Items.");
        }

        public async Task<Response<IEnumerable<WishlistItemsDto>>> GetItemsInWishlist(int listId)
        {
            var list = await _unitOfWork.Wishlists.GetByIdAsync(listId);
            if (list != null)
            {
                var items = await _unitOfWork.WishlistItems.GetTableNoTracking()
                    .Where(i => i.WishlistId == listId)
                    .Include(i => i.Wishlist).Include(i => i.Product)
                    .ToListAsync();

                if (items != null && items.Count > 0)
                {
                    var dto = _mapper.Map<IEnumerable<WishlistItemsDto>>(items);
                    return Success(dto);
                }
                return NotFound<IEnumerable<WishlistItemsDto>>("There is no Items.");
            }
            return NotFound<IEnumerable<WishlistItemsDto>>("Wishlift not found.");
        }

        public async Task<Response<WishlistItemsDto>> GetWishlistItem(int id)
        {
            var item = await _unitOfWork.WishlistItems.GetByIdAsync(id);

            if (item != null)
            {
                var dto = _mapper.Map<WishlistItemsDto>(item);
                return Success(dto);

            }
            return NotFound<WishlistItemsDto>("Item not found.");
        }

        public async Task<Response<string>> UpdateWishlistItem(int id, WishlistItem item)
        {
            var prevItem = await _unitOfWork.WishlistItems.GetTableNoTracking().Where(o => o.Id == id).Include(i => i.Wishlist).Include(i => i.Product).FirstOrDefaultAsync();
            if (prevItem == null)
            {
                return NotFound<string>("This Item not found.");
            }
            var existingItem = await _unitOfWork.Wishlists.GetByIdAsync(item.WishlistId);

            if (existingItem == null)
            {
                return BadRequest<string>("Wish list you try to add item to is not found.");
            }
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(item.ProductId);

            if (existingProduct == null)
            {
                return BadRequest<string>("Product you try to add is not found.");
            }

            item.Id = id;
            item.Wishlist = existingItem;
            item.Product = existingProduct;
            await _unitOfWork.WishlistItems.UpdateAsync(item);
            return Updated<string>("Item updated successfully.");
        }
    }
}
