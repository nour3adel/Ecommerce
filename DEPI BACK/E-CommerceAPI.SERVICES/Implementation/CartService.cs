using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.CartDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class CartService : ResponseHandler, ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<string>> AddCart(ApplicationUser currentUser)
        {
            if (currentUser == null)
            {
                return BadRequest<string>("User information is required to create a cart.");
            }

            // Check if user already has an open cart
            var existingCart = await _unitOfWork.Carts
                .GetTableNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == currentUser.Id && !c.IsClosed);

            if (existingCart != null)
            {
                return BadRequest<string>("You already have an open cart. Please close it before creating a new one.");
            }

            CartDto dto = new()
            {
                CustomerId = currentUser.Id
            };

            var cart = _mapper.Map<Cart>(dto);

            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.Save();

            return Success("Cart added successfully.");
        }

        public async Task<Response<string>> DeleteCart(int id)
        {
            var cart = await _unitOfWork.Carts.GetByIdAsync(id);
            if (cart != null)
            {
                await _unitOfWork.Carts.DeleteAsync(cart);
                await _unitOfWork.Save();

                return Deleted<string>("Cart deleted successfully.");
            }
            return NotFound<string>("Cart not found.");
        }
        public async Task<Response<string>> DeleteCurrentUserCart(ApplicationUser currentUser)
        {
            if (currentUser == null)
            {
                return BadRequest<string>("User information is required.");
            }

            // Fetch the user's open cart
            var openCart = await _unitOfWork.Carts
                .GetTableNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == currentUser.Id && !c.IsClosed);

            if (openCart == null)
            {
                return NotFound<string>("No open cart found for the user.");
            }

            // Delete the cart itself
            await _unitOfWork.Carts.DeleteAsync(openCart);

            // Save all changes
            await _unitOfWork.Save();

            return Deleted<string>("Cart deleted successfully.");
        }


        public async Task<Response<IEnumerable<CartDto>>> GetAllCarts()
        {
            var carts = await _unitOfWork.Carts.GetTableNoTracking().ToListAsync();
            if (carts != null && carts.Count > 0)
            {
                return Success(_mapper.Map<IEnumerable<CartDto>>(carts));
            }

            return NotFound<IEnumerable<CartDto>>("There are no carts yet.");
        }

        public async Task<Response<CartDto>> GetCart(int id)
        {
            var cart = await _unitOfWork.Carts.GetByIdAsync(id);
            if (cart != null)
            {
                return Success(_mapper.Map<CartDto>(cart));
            }
            return NotFound<CartDto>("This cart does not exist.");
        }


    }
}
