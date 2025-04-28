using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Models.Product;

namespace Ecommerce.Frontend.Services.Products
{
    public interface IProductService
    {
        Task<Response<List<ProductDTO>>> GetProductsAsync();
        Task<Response<ProductDTO>> GetProductByIdAsync(int id);
        Task<Response<string>> AddProductAsync(AddProductDto product);
    }
}
