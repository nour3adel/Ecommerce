using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.Contracts;
using TechXpress.Data.Repositories;

namespace TechXpress.Data.DataContext
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly TechXpressDbContext _context;

        private readonly Lazy<IProductRepository> _products;

        private readonly Lazy<ICategoryRepository> _categories;

        private readonly Lazy<IOrderRepository> _orders;


        public UnitOfWork(TechXpressDbContext context) {
            _context = context;
            _products = new Lazy<IProductRepository>(new ProductRepository(_context));
            _categories = new Lazy<ICategoryRepository>(new CategoryRepository(_context));
            _orders = new Lazy<IOrderRepository>(new OrderRepository(_context));

        
        }
        public IProductRepository Products => _products.Value;

        public ICategoryRepository Categories => _categories.Value;

        public IOrderRepository Orders => _orders.Value;

        

        public void SaveChanges()
        {
           _context.SaveChanges();
        }
    }
}
