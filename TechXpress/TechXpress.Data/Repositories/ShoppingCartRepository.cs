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
    internal class ShoppingCartRepository : RepositoryBase<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(TechXpressDbContext dbContext) : base(dbContext)
        {
        }
    }
}
