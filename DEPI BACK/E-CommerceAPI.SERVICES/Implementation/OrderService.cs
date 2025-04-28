using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.OrderDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class OrderService : ResponseHandler, IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;

            _unitOfWork = unitOfWork;
        }

        public async Task<Response<string>> AddOrder(ApplicationUser currentUser, OrderDto dto)
        {
            if (currentUser == null)
            {
                return BadRequest<string>("User information is required to create a cart.");
            }
            // Check if user already has an open cart
            var existingOrder = await _unitOfWork.Orders
                .GetTableNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == currentUser.Id && c.Status == "opened");

            if (existingOrder != null)
            {
                return BadRequest<string>("You already have an open order. Please close it before creating a new one.");
            }

            var order = _mapper.Map<Order>(dto);

            if (currentUser != null)
                order.CustomerId = currentUser.Id;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.Save();
            return Success("Order added successfully.");
        }

        public async Task<Response<string>> DeleteOrder(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order != null)
            {
                var items = await _unitOfWork.OrderItems.GetItemsInOrder(id);

                await _unitOfWork.OrderItems.DeleteRangeAsync(items);
                await _unitOfWork.Orders.DeleteAsync(order);
                await _unitOfWork.Save();
                return Deleted<string>("Order deleted successfully.");


            }
            return BadRequest<string>("This order already is not exist.");
        }
        public async Task<Response<string>> DeleteCurrentUserOrder(ApplicationUser currentUser)
        {
            if (currentUser == null)
            {
                return BadRequest<string>("User information is required.");
            }

            // Fetch the user's open cart
            var openOrder = await _unitOfWork.Orders
                .GetTableNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == currentUser.Id && c.Status == "opened");

            if (openOrder == null)
            {
                return NotFound<string>("No open order found for the user.");
            }

            // Delete the cart itself
            await _unitOfWork.Orders.DeleteAsync(openOrder);

            // Save all changes
            await _unitOfWork.Save();

            return Deleted<string>("Order deleted successfully.");
        }

        public async Task<Response<IEnumerable<OrderDto>>> GetCustomerOrders()
        {
            var orders = await _unitOfWork.Orders.GetTableNoTracking().ToListAsync();
            if (orders != null && orders.Count > 0)
            {
                var dto = _mapper.Map<IEnumerable<OrderDto>>(orders);
                return Success(dto);
            }
            return BadRequest<IEnumerable<OrderDto>>("There is no Orders.");

        }

        public async Task<Response<OrderDto>> GetOrderById(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound<OrderDto>("Order Not Found!");
            }

            var dto = _mapper.Map<OrderDto>(order);
            return Success(dto);
        }

        public async Task<Response<string>> UpdateOrder(int id, OrderDto dto)
        {
            var result = await _unitOfWork.Orders.GetByIdAsync(id);
            if (result != null)
            {
                var order = _mapper.Map<Order>(dto);
                order.Id = id;
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.Save();
                return Updated<string>("Order updated successfully.");
            }
            return NotFound<string>("Order not found.");

        }

    }
}
