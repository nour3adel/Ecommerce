using E_CommerceAPI.ENTITES.DTOs.ProductDTO;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IProductService
    {
        public Task<Response<IEnumerable<ProductDto>>> GetAllProducts();
        public Task<Response<ProductDto>> GetProductById(int id);
        public Task<Response<ProductDto>> GetProductByName(string name);
        public Task<Response<ProductDetailedDto>> GetProductsDetailedByID(int id);
        public Task<Response<IEnumerable<ProductDetailedDto>>> GetAllProductsDetiled();
        public Task<Response<IEnumerable<ProductDto>>> GetProductsByCategoryId(int idd);
        public Task<Response<IEnumerable<ProductDto>>> GetProductsByCategoryName(string name);
        public Task<Response<IEnumerable<ProductDto>>> GetProductsByBrandId(int id);
        public Task<Response<IEnumerable<ProductDto>>> GetProductsByBrandName(string name);

        public Task<Response<string>> AddProductAsync(AddProductDto dto);
        public Task<Response<string>> DeleteProductAsync(int id);
        public Task<Response<string>> UpdateProductAsync(int id, AddProductDto dto);
    }
}
