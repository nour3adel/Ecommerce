using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.Contracts;
using TechXpress.Data.Models;
using TechXpress.Services.Contracts;
using TechXpress.Services.DTOs.OrderDtos;

namespace TechXpress.Services.Services
{
    internal class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        private IOrderRepository _orders;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orders = _unitOfWork.Orders;

        }
        public bool DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public OrderListDto GetOrder(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrderListDto> GetOrderList()
        {
            throw new NotImplementedException();
        }

        public bool InsertOrder(OrderInsertDto order)
        {
            throw new NotImplementedException();
            //Order order = new Order
            //{

            //};
            
        }

        public bool UpdateOrder(OrderUpdateDto order)
        {
            throw new NotImplementedException();
        }
    }
}
