using E_CommerceAPI.ENTITES.DTOs.OrderDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IOrderItemService
    {
        public Task<Response<OrderItemDto>> GetOrderItem(int id);
        public Task<Response<IEnumerable<OrderItemDto>>> GetAllItems(ApplicationUser applicationUser);
        public Task<Response<IEnumerable<OrderItemDto>>> GetItemsInOrder(int orderId);

        public Task<Response<string>> AddOrderItem(AddOrderItemDto item, ApplicationUser applicationUser);

        public Task<Response<string>> UpdateOrderItem(int id, AddOrderItemDto item, ApplicationUser applicationUser);

        public Task<Response<string>> DeleteOrderItem(int id);
    }
}
