using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Services.DTOs.CategoryDtos;

namespace TechXpress.Services.Contracts
{
    public interface ICategoryService
    {
        CategoryListDto GetCategory(int id);

        IEnumerable<CategoryListDto> GetCategoryList();

        bool InsertCategory(CategoryInsertDto category);

        bool UpdateCategory(CategoryUpdateDto category);

        bool DeleteCategory(int id);
    }
}
