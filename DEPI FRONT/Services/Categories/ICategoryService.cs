using Ecommerce.Frontend.Models.Category;
using Ecommerce.Frontend.Models.Common;

namespace Ecommerce.Frontend.Services.Categories
{
    public interface ICategoryService
    {
        Task<Response<List<CategoryDto>>> GetCategoriessAsync();
        Task<Response<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<Response<string>> AddCategoryAsync(AddCategoryDto category);
    }
}
