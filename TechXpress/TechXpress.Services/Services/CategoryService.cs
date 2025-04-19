using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.Contracts;
using TechXpress.Services.Contracts;
using TechXpress.Services.DTOs.CategoryDtos;


namespace TechXpress.Services.Services
{
    internal class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        private ICategoryRepository _categories;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _categories = _unitOfWork.Categories;
        }

        public bool DeleteCategory(int id)
        {
            throw new NotImplementedException();
        }

        public CategoryListDto GetCategory(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CategoryListDto> GetCategoryList()
        {
            throw new NotImplementedException();
        }

        public bool InsertCategory(CategoryInsertDto category)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCategory(CategoryUpdateDto category)
        {
            throw new NotImplementedException();
        }
    }
}
