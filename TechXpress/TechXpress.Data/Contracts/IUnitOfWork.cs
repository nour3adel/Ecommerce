using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TechXpress.Data.Models;

namespace TechXpress.Data.Contracts
{
    public interface IUnitOfWork
    {

        UserManager<AppUser> UserManager { get; }

        IProductRepository Products { get; }

        ICategoryRepository Categories { get; }

        IOrderRepository Orders { get; }

        IShoppingCartRepository ShoppingCarts { get; }

        void SaveChanges();
    }
}
