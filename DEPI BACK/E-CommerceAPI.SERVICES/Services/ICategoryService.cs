using E_CommerceAPI.ENTITES.DTOs.CategoryDTO;
using E_CommerceAPI.SERVICES.Bases;


namespace E_CommerceAPI.SERVICES.Services
{
    public interface ICategoryService
    {
        public Task<Response<IEnumerable<CategoryDto>>> GetAllCategories();
        public Task<Response<IEnumerable<CategoryDetailedDto>>> GetAllCategoriesDetailed();
        public Task<Response<CategoryDetailedDto>> GetAllCategoriesDetailedByID(int id);
        public Task<Response<CategoryDto>> GetCategoryById(int id);
        public Task<Response<CategoryDto>> GetCategoryByName(string name);

        public Task<Response<string>> AddCategory(AddCategoryDto dto);
        public Task<Response<string>> UpdateCategory(int id, AddCategoryDto dto);
        public Task<Response<string>> DeleteCategory(int id);
    }
}
