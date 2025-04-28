using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;

namespace E_CommerceAPI.Infrastructure.Interfaces
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        public Task<Brand> GetBrandByName(string name);
    }
}
