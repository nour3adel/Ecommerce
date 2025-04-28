using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;

namespace E_CommerceAPI.Infrastructure.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public Task<Category> GetCategoryByName(string name);
    }
}
