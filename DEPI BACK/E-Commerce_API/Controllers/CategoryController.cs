using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.CategoryDTO;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Categorys")]

    public class CategoryController : AppControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet("Detailed")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _categoryService.GetAllCategories();
                    return NewResult(response);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all categories.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesDetailed()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _categoryService.GetAllCategoriesDetailed();
                    return NewResult(response);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all detailed categories.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        [HttpGet("Detailed/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _categoryService.GetCategoryById(id);
                    return NewResult(response);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching category with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryDetailedById(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _categoryService.GetAllCategoriesDetailedByID(id);
                    return NewResult(response);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching detailed category with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        [HttpGet("Name/{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _categoryService.GetCategoryByName(name);
                    return NewResult(response);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching category with name {name}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(AddCategoryDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _categoryService.AddCategory(dto);
                    return NewResult(response);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a category.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> EditCategory(int id, AddCategoryDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _categoryService.UpdateCategory(id, dto);
                    return NewResult(response);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating category with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _categoryService.DeleteCategory(id);
                    return NewResult(response);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting category with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

    }
}
