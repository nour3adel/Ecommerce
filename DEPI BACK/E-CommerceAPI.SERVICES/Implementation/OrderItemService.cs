using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.OrderDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class OrderItemService : ResponseHandler, IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<Response<string>> AddOrderItem(AddOrderItemDto item, ApplicationUser currentUser)
        {
            if (currentUser == null)
            {
                return BadRequest<string>("User information is required.");
            }
            // Step 1: Get the user's open cart
            var orderExist = await _unitOfWork.Orders
                .GetTableNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == currentUser.Id && c.Status == "opened");


            if (orderExist == null)
            {
                return NotFound<string>("You don't have an open order. Please create one first.");

            }
            // Step 2: Validate if the product exists
            var productExist = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (productExist == null)
            {
                return BadRequest<string>("The Product you try to add is not exist.");
            }
            var existingOrderItem = await _unitOfWork.OrderItems
                                     .GetTableNoTracking()
                                     .FirstOrDefaultAsync(ci => ci.OrderId == orderExist.Id && ci.ProductId == item.ProductId);
            if (existingOrderItem != null)
            {
                // Increment the quantity
                existingOrderItem.Quantity += item.Quantity;
                await _unitOfWork.OrderItems.UpdateAsync(existingOrderItem);
            }
            else
            {

                var orderItem = new OrderItem
                {
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Order = orderExist, // Set the navigation property for the cart
                    Product = productExist // Set the navigation property for the product

                };
                await _unitOfWork.OrderItems.AddAsync(orderItem);


            }

            await _unitOfWork.Save();

            return Success("orderitem added to order successfully.");

        }

        public async Task<Response<string>> DeleteOrderItem(int id)
        {
            var item = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (item != null)
            {
                await _unitOfWork.OrderItems.DeleteAsync(item);
                await _unitOfWork.Save();
                return Deleted<string>("Item deleted successfully.");

            }
            return NotFound<string>("This Order item is not exist.");
        }
        public async Task<Response<IEnumerable<OrderItemDto>>> GetAllItems(ApplicationUser applicationUser)
        {
            var orders = await _unitOfWork.OrderItems.GetTableNoTracking()
               .Include(o => o.Product).ToListAsync();

            if (orders != null && orders.Count > 0)
            {
                var dto = _mapper.Map<IEnumerable<OrderItemDto>>(orders);
                return Success(dto);
            }
            return BadRequest<IEnumerable<OrderItemDto>>("There is no Items yet!");
        }

        public async Task<Response<IEnumerable<OrderItemDto>>> GetItemsInOrder(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order != null)
            {
                order.OrderItems = await _unitOfWork.OrderItems.GetItemsInOrder(orderId);

                if (order.OrderItems != null && order.OrderItems.Count > 0)
                {
                    var dto = _mapper.Map<IEnumerable<OrderItemDto>>(order.OrderItems);
                    return Success(dto);
                }
                return NotFound<IEnumerable<OrderItemDto>>("There is no items yet in this order.");
            }
            return NotFound<IEnumerable<OrderItemDto>>("TThis order is not exist.");
        }

        public async Task<Response<OrderItemDto>> GetOrderItem(int id)
        {
            var order = await _unitOfWork.OrderItems.GetByIdAsync(id);

            if (order != null)
            {
                var dto = _mapper.Map<OrderItemDto>(order);
                return Success(dto);
            }
            return NotFound<OrderItemDto>("This Order item is not exist.");
        }

        public async Task<Response<string>> UpdateOrderItem(int id, AddOrderItemDto itemDto, ApplicationUser currentUser)
        {
            // Step 1: Retrieve the existing CartItem from the database
            if (currentUser == null)
            {
                return BadRequest<string>("User information is required.");
            }
            // Step 1: Retrieve the existing OrderItem from the database
            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (orderItem == null)
            {
                return NotFound<string>("This Order Item does not exist.");
            }
            // Step 2: Get the user's open cart
            var openOrder = await _unitOfWork.Orders
                .GetTableNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == currentUser.Id && c.Status == "opened");

            if (openOrder == null)
            {
                return BadRequest<string>("You don't have an open order to update an item in.");
            }

            // Step 3: Validate the associated Product
            var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
            if (product == null)
            {
                return BadRequest<string>("The Product you are trying to add does not exist.");
            }
            // Step 4: Validate Quantity
            if (itemDto.Quantity <= 0)
            {
                return BadRequest<string>("Quantity must be greater than zero.");
            }

            // Step 5: Check if the cart item belongs to the user's open cart
            if (orderItem.OrderId != orderItem.Id)
            {
                return BadRequest<string>("This order item does not belong to your open cart.");
            }
            // Step 4: Update the OrderItem properties
            orderItem.OrderId = itemDto.OrderId;
            orderItem.ProductId = itemDto.ProductId;
            orderItem.Quantity = itemDto.Quantity;

            // Step 5: Update the OrderItem in the database
            await _unitOfWork.OrderItems.UpdateAsync(orderItem);
            await _unitOfWork.Save();

            return Updated<string>("Order Item updated successfully.");
        }
    }

}
