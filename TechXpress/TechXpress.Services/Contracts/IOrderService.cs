using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Services.DTOs.OrderDtos;

namespace TechXpress.Services.Contracts
{
    public interface IOrderService
    {
        OrderListDto GetOrder(int id);

        IEnumerable<OrderListDto> GetOrderList();

        bool InsertOrder(OrderInsertDto order);

        bool UpdateOrder(OrderUpdateDto order);

        bool DeleteOrder(int id);
    }
}
