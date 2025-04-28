using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ECommerceDbContext _context;
        public CategoryRepository(ECommerceDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<Category> GetCategoryByName(string name)
        {
            var category = await _context.Categories.Where(c => c.Name == name).FirstOrDefaultAsync();
            return category;
        }
    }
}
