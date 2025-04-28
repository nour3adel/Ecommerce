using E_CommerceAPI.ENTITES.DTOs.OrderDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IOrderService
    {
        public Task<Response<OrderDto>> GetOrderById(int id);
        public Task<Response<IEnumerable<OrderDto>>> GetCustomerOrders();

        public Task<Response<string>> AddOrder(ApplicationUser currentUser, OrderDto dto);

        public Task<Response<string>> UpdateOrder(int id, OrderDto dto);

        public Task<Response<string>> DeleteOrder(int id);
        public Task<Response<string>> DeleteCurrentUserOrder(ApplicationUser applicationUser);

    }
}

