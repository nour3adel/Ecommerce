using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.Contracts;
using TechXpress.Data.Models;
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
            try
            {
                _categories.Delete(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            catch
            { 
                return false;
            }
        }

        public CategoryListDto GetCategory(int id)
        {
            var category = _categories.GetById(id);
            return new CategoryListDto
            {
                Id = category.Id,
                Name= category.Name,
                Description= category.Description,
            };
        }

        public IEnumerable<CategoryListDto> GetCategoryList()
        {
            return _categories.GetAll()
                .Select(cat => new CategoryListDto
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    Description = cat.Description,
                });
        }

        public bool InsertCategory(CategoryInsertDto category)
        {
            try
            {
                Category cat = new Category
                {
                    Name = category.Name,
                    Description = category.Description,
                };
                _categories.Insert(cat);
                _unitOfWork.SaveChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }

        public bool UpdateCategory(CategoryUpdateDto category)
        {
            try
            {
                Category cat = new Category
                {
                    Name = category.Name,
                    Description = category.Description,
                };
                _categories.Update(cat);
                _unitOfWork.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
