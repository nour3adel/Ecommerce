using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace E_CommerceAPI.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ECommerceDbContext _context;
        public ProductRepository(ECommerceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByName(string name)
        {
            var product = await _context.Products.Where(p => p.Name == name)
                .Include(p => p.Brand).Include(p => p.Category)
                .FirstOrDefaultAsync();

            return product;

        }

        public async Task<IEnumerable<Product>> GetProductsByBrandId(int id)
        {
            var products = await _context.Products.Where(p => p.BrandId == id)
                .Include(p => p.Category).Include(p => p.Brand)
                .ToListAsync();
            return products;
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandName(string name)
        {
            var products = await _context.Products.Where(p => p.Brand!.Name == name)
                .Include(p => p.Category).Include(p => p.Brand)
                .ToListAsync();
            return products;
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryId(int catId)
        {
            var products = await _context.Products.Where(p => p.CategoryId == catId)
                .Include(p => p.Category).Include(p => p.Brand)
                .ToListAsync();
            return products;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryName(string name)
        {
            var products = await _context.Products.Where(p => p.Category!.Name == name)
                .Include(p => p.Category).Include(p => p.Brand)
                .ToListAsync();
            return products;
        }






    }
}
