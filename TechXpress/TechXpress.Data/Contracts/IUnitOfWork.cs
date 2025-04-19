using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress.Data.Contracts
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }

        ICategoryRepository Categories { get; }

        IOrderRepository Orders { get; }

        void SaveChanges();
    }
}
