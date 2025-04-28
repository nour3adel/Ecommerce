using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.CartDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class CartItemsService : ResponseHandler, ICartItemsService
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CartItemsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<IEnumerable<CartItemsDto>>> GetAllCartsItems(ApplicationUser currentUser)
        {
            // Early return if user is null
            if (currentUser == null)
            {
                return BadRequest<IEnumerable<CartItemsDto>>("User information is required to create a cart.");
            }

            // Fetch the items with the related entities loaded
            var items = await _unitOfWork.CartItems.GetTableNoTracking()
                .Include(x => x.Cart)
                .Include(x => x.Product)
                    .ThenInclude(p => p.Brand)  // Improved to use ThenInclude for cleaner code
                .Include(x => x.Product.Category)
                .Include(x => x.Product.Images)
                .Where(x => x.Cart.CustomerId == currentUser.Id && !x.Cart.IsClosed)
                .ToListAsync();

            // If no items found, return NotFound
            if (!items.Any())
            {
                return NotFound<IEnumerable<CartItemsDto>>("There are no items in this cart.");
            }

            // Map the fetched items directly to DTOs
            var dto = items.Select(x => new CartItemsDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductBrand = x.Product.Brand?.Name,  // Safely access Brand.Name in case it's null
                ProductCategory = x.Product.Category?.Name, // Same for Category.Name
                ProductImage = x.Product.Images?.FirstOrDefault()?.ImageUrl, // Safely handle the first image
                Quantity = x.Quantity,
                ProductName = x.Product.Name,
                ProductPrice = x.Product.Price,
            });

            // Return success response with mapped DTOs
            return Success(dto);
        }



        public async Task<Response<CartItemsDto>> GetCartItem(int id)
        {
            var item = await _unitOfWork.CartItems.GetByIdAsync(id);

            if (item != null)
            {
                var dto = _mapper.Map<CartItemsDto>(item);
                return Success(dto);
            }
            return NotFound<CartItemsDto>("Item not found.");
        }

        //public async Task<Response<List<CartItemsDto>>> GetItemsInCart(int cartId)
        //{
        //    var items = await _unitOfWork.CartItems.GetItemsByCartID(cartId);

        //    if (items != null && items.Count() > 0)
        //    {
        //        var dto = _mapper.Map<List<CartItemsDto>>(items);
        //        return Success(dto);

        //    }

        //    return NotFound<List<CartItemsDto>>("There is no items in this cart.");
        //}

        public async Task<Response<string>> AddItemToCart(AddCartItemDto itemDto, ApplicationUser currentUser)
        {
            if (currentUser == null)
            {
                return BadRequest<string>("User information is required.");
            }

            // Step 1: Get the user's open cart
            var openCart = await _unitOfWork.Carts
                .GetTableNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == currentUser.Id && !c.IsClosed);

            if (openCart == null)
            {
                return NotFound<string>("You don't have an open cart. Please create one first.");
            }

            // Step 2: Validate if the product exists
            var productExist = await _unitOfWork.Products.GetByIdAsync(itemDto.Productid);
            if (productExist == null)
            {
                return NotFound<string>("The product you try to add does not exist.");
            }
            // Step 3: Check if the product is already in the cart
            var existingCartItem = await _unitOfWork.CartItems
                .GetTableNoTracking()
                .FirstOrDefaultAsync(ci => ci.CartId == openCart.Id && ci.ProductId == itemDto.Productid);

            if (existingCartItem != null)
            {
                // Increment the quantity
                existingCartItem.Quantity += itemDto.Quantity;
                await _unitOfWork.CartItems.UpdateAsync(existingCartItem);
            }
            else
            {

                // Step 3: Map DTO to CartItem entity
                var cartItem = new CartItem
                {
                    CartId = openCart.Id,
                    ProductId = itemDto.Productid,
                    Quantity = itemDto.Quantity,
                    Product = productExist
                };
                await _unitOfWork.CartItems.AddAsync(cartItem);

            }

            // Step 4: Add to database
            await _unitOfWork.Save();

            return Success("Item added to cart successfully.");
        }


        public async Task<Response<string>> UpdateCartItem(AddCartItemDto itemDto, ApplicationUser currentUser)
        {
            // Step 1: Validate user and item data
            if (currentUser == null)
            {
                return BadRequest<string>("User information is required.");
            }

            if (itemDto == null || itemDto.Quantity <= 0)
            {
                return BadRequest<string>("Invalid or missing cart item data.");
            }

            // Step 2: Get user's open cart
            var openCart = await _unitOfWork.Carts.GetTableNoTracking().FirstOrDefaultAsync(c => c.CustomerId == currentUser.Id && !c.IsClosed);
            if (openCart == null)
            {
                return BadRequest<string>("You don't have an open cart to update an item in.");
            }

            // Step 3: Validate product
            var product = await _unitOfWork.Products.GetTableNoTracking().FirstOrDefaultAsync(p => p.Id == itemDto.Productid);
            if (product == null)
            {
                return BadRequest<string>("The product you are trying to update does not exist.");
            }

            // Step 4: Retrieve the specific cart item
            var cartItem = await _unitOfWork.CartItems.GetTableAsTracking().FirstOrDefaultAsync(ci =>
                ci.CartId == openCart.Id && ci.ProductId == itemDto.Productid);

            if (cartItem == null)
            {
                return BadRequest<string>("Cart item not found in your open cart.");
            }

            // Step 5: Update the cart item
            cartItem.Quantity = itemDto.Quantity;

            try
            {
                await _unitOfWork.CartItems.UpdateAsync(cartItem);
                await _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return BadRequest<string>($"An error occurred while updating the cart item: {ex.Message}");
            }

            // Return success response
            return Success<string>("Cart item updated successfully.");
        }


        public async Task<Response<string>> DeleteItemFromCart(int id)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(id);

            if (cartItem == null)
            {
                return NotFound<string>("Cart Item not found.");
            }

            await _unitOfWork.CartItems.DeleteAsync(cartItem);
            await _unitOfWork.Save();

            return Success("Cart item deleted successfully.");

        }
    }
}
