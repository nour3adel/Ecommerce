using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TechXpress.Data.Contracts;
using TechXpress.Data.Models;
using TechXpress.Data.Repositories;

namespace TechXpress.Data.DataContext
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly TechXpressDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        private readonly Lazy<IProductRepository> _products;

        private readonly Lazy<ICategoryRepository> _categories;

        private readonly Lazy<IOrderRepository> _orders;

        private readonly Lazy<IShoppingCartRepository> _shoppingCarts;


        public UnitOfWork(TechXpressDbContext context,UserManager<AppUser> userManager) {
            _context = context;
            _userManager = userManager;
            _products = new Lazy<IProductRepository>(new ProductRepository(_context));
            _categories = new Lazy<ICategoryRepository>(new CategoryRepository(_context));
            _orders = new Lazy<IOrderRepository>(new OrderRepository(_context));
            _shoppingCarts = new Lazy<IShoppingCartRepository>(new ShoppingCartRepository(_context));

        
        }
        public IProductRepository Products => _products.Value;

        public ICategoryRepository Categories => _categories.Value;

        public IOrderRepository Orders => _orders.Value;

        public UserManager<AppUser> UserManager => _userManager;

        public IShoppingCartRepository ShoppingCarts => _shoppingCarts.Value;

        public void SaveChanges()
        {
           _context.SaveChanges();
        }
    }
}
