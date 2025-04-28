using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.CategoryDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class CategoryService : ResponseHandler, ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper,
            IFileService fileService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<string>> AddCategory(AddCategoryDto dto)
        {
            var categoryExist = await _unitOfWork.Categories.GetCategoryByName(dto.Name);
            if (categoryExist != null)
            {
                return BadRequest<string>("This category is already exists.");

            }

            var category = _mapper.Map<Category>(dto);
            var context = _httpContextAccessor.HttpContext;
            if (context?.Request == null)
            {
                return BadRequest<string>("HttpContext or Request is not available.");
            }

            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

            if (dto.ImageFile != null)
            {
                var imageUrl = await _fileService.UploadImage("Categories", dto.ImageFile);
                switch (imageUrl)
                {
                    case "NoImage":
                        return NotFound<string>("Image was not found.");
                    case "FailedToUploadImage":
                        return BadRequest<string>("Failed to upload image.");
                    case "ImageSizeExceeded":
                        return BadRequest<string>("Image size exceeds the maximum limit of 1 MB.");
                    case "InvalidFileType":
                        return BadRequest<string>("Invalid image file type.");
                    default:
                        category.Image = $"{baseUrl}{imageUrl}";
                        break;
                }

            }
            var result = await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.Save();
            return Success("Category added successfully.");

        }

        public async Task<Response<string>> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                if (!string.IsNullOrEmpty(category.Image))
                {
                    var imagePath = category.Image;
                    _fileService.DeleteImage(imagePath);
                }
                await _unitOfWork.Categories.DeleteAsync(category);
                await _unitOfWork.Save();
                return Deleted<string>("Category Deleted successfully.");

            }
            return BadRequest<string>("This category is not exist.");
        }

        public async Task<Response<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _unitOfWork.Categories.GetTableNoTracking().ToListAsync();
            if (categories != null && categories.Count > 0)
            {
                var dto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
                return Success(dto);
            }
            return NotFound<IEnumerable<CategoryDto>>("There is no categories yet.");
        }
        public async Task<Response<IEnumerable<CategoryDetailedDto>>> GetAllCategoriesDetailed()
        {
            var categories = await _unitOfWork.Categories
                .GetTableNoTracking()
                .Include(c => c.Products)
                .ToListAsync();

            if (categories == null || categories.Count == 0)
                return NotFound<IEnumerable<CategoryDetailedDto>>("There are no categories yet.");

            var result = categories.Select(c => new CategoryDetailedDto
            {
                Id = c.Id,
                Name = c.Name,
                Image = c.Image,
                TotalStock = c.Products?.Sum(p => p.StockAmount) ?? 0
            });

            return Success(result);
        }
        public async Task<Response<CategoryDetailedDto>> GetAllCategoriesDetailedByID(int id)
        {
            var categoriy = await _unitOfWork.Categories.GetByIdAsync(id);
            if (categoriy != null)
            {
                var result = new CategoryDetailedDto
                {
                    Id = categoriy.Id,
                    Name = categoriy.Name,
                    Image = categoriy.Image,
                    TotalStock = categoriy.Products?.Sum(p => p.StockAmount) ?? 0
                };
                return Success(result);
            }
            return NotFound<CategoryDetailedDto>("This category is not exist.");
        }


        public async Task<Response<CategoryDto>> GetCategoryById(int id)
        {
            var categoriy = await _unitOfWork.Categories.GetByIdAsync(id);
            if (categoriy != null)
            {
                var dto = _mapper.Map<CategoryDto>(categoriy);
                return Success(dto);
            }
            return NotFound<CategoryDto>("This category is not exist.");
        }

        public async Task<Response<CategoryDto>> GetCategoryByName(string name)
        {
            var categoriy = await _unitOfWork.Categories.GetCategoryByName(name);
            if (categoriy != null)
            {
                var dto = _mapper.Map<CategoryDto>(categoriy);
                return Success(dto);
            }
            return NotFound<CategoryDto>("This category is not exist.");
        }

        public async Task<Response<string>> UpdateCategory(int id, AddCategoryDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                var context = _httpContextAccessor.HttpContext;
                if (context?.Request == null)
                {
                    return BadRequest<string>("HttpContext or Request is not available.");
                }

                var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
                if (dto.ImageFile != null)
                {
                    var imageUrl = await _fileService.UploadImage("Categories", dto.ImageFile);

                    switch (imageUrl)
                    {
                        case "NoImage":
                            return NotFound<string>("Image was not found.");
                        case "FailedToUploadImage":
                            return BadRequest<string>("Failed to upload image.");
                        case "ImageSizeExceeded":
                            return BadRequest<string>("Image size exceeds the maximum limit of 1 MB.");
                        case "InvalidFileType":
                            return BadRequest<string>("Invalid image file type.");
                        default:
                            if (!string.IsNullOrEmpty(category.Image))
                            {
                                _fileService.DeleteImage(category.Image);
                            }
                            category.Image = $"{baseUrl}{imageUrl}";
                            break;
                    }
                }
                var result = _mapper.Map<Category>(dto);
                await _unitOfWork.Categories.UpdateAsync(category);
                await _unitOfWork.Save();

                return Updated<string>("Category updated successfully.");
            }
            return NotFound<string>("This category is not exist.");
        }
    }
}
