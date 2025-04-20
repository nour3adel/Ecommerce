using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Services.Contracts;
using TechXpress.Services.DTOs.CategoryDtos;

namespace TechXpress.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_categoryService.GetCategoryList());
        }

        [HttpPost]

        public IActionResult Post(CategoryInsertDto category)
        {
            return Ok(_categoryService.InsertCategory(category));
        }

        [HttpPut]
        public IActionResult Put(CategoryUpdateDto category)
        {
            return Ok(_categoryService.UpdateCategory(category));
        }



    }
}
