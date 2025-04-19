using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.Contracts;
using TechXpress.Data.DataContext;
using TechXpress.Data.Models;

namespace TechXpress.Data.Repositories
{
    internal class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(TechXpressDbContext dbContext) : base(dbContext)
        {
        }
    }
}
