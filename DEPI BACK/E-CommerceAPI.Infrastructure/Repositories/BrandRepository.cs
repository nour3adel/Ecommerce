using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace E_CommerceAPI.Infrastructure.Repositories
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {

        private readonly ECommerceDbContext _context;

        public BrandRepository(ECommerceDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<Brand> GetBrandByName(string name)
        {
            var brand = await _context.Brands.Where(b => b.Name == name).FirstOrDefaultAsync();
            return brand;
        }

    }
}
