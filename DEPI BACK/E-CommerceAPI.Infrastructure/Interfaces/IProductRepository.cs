using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;


namespace E_CommerceAPI.Infrastructure.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public Task<Product> GetProductByName(string name);
        public Task<IEnumerable<Product>> GetProductsByCategoryId(int idd);
        public Task<IEnumerable<Product>> GetProductsByCategoryName(string name);
        public Task<IEnumerable<Product>> GetProductsByBrandId(int id);
        public Task<IEnumerable<Product>> GetProductsByBrandName(string name);


    }
}
