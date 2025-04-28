using Ecommerce.Frontend.Models.Brand;
using Ecommerce.Frontend.Models.Common;

namespace Ecommerce.Frontend.Services.Brands
{
    public interface IBrandService
    {
        Task<Response<List<BrandDto>>> GetBrandsAsync();
        Task<Response<BrandDto>> GetBrandByIdAsync(int id);
        Task<Response<string>> AddBrandAsync(AddBrandDto category);
    }
}
