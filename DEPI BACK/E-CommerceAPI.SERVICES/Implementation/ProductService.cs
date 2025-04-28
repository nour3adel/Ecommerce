using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.ProductDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class ProductService : ResponseHandler, IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private List<string> Extentions = new() { ".jpg", ".png", ".jpeg" };

        public ProductService(IUnitOfWork unitOfWork,
                              IMapper mapper,
                              IFileService fileService,
                              IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;


        }


        public async Task<Response<string>> AddProductAsync(AddProductDto dto)
        {
            // Validate Category
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return BadRequest<string>("Invalid category ID.");

            // Validate Brand
            var brand = await _unitOfWork.Brands.GetByIdAsync(dto.BrandId);
            if (brand == null)
                return BadRequest<string>("Invalid brand ID.");

            // Map DTO to Product entity
            var product = _mapper.Map<Product>(dto);

            // Handle Image Upload
            if (dto.ImageFiles?.Count > 0)
            {
                var imageValidationResult = await ValidateAndUploadImages(dto.ImageFiles, category.Name);
                if (!imageValidationResult.Succeeded)
                    return BadRequest<string>(imageValidationResult.Message);

                product.Images = imageValidationResult.Data
                    .Select(url => new ProductImage { ImageUrl = url })
                    .ToList();
            }

            // Handle Colors
            if (dto.ColorNames?.Count > 0)
            {
                product.Colors = dto.ColorNames
                    .Where(color => !string.IsNullOrWhiteSpace(color))
                    .Select(color => new ProductColor { ColorName = color.Trim() })
                    .ToList();
            }

            // Save to DB
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.Save();

            return Success("Product added successfully.");
        }


        private async Task<Response<List<string>>> ValidateAndUploadImages(List<IFormFile> imageFiles, string categoryName)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var maxFileSize = 2 * 1024 * 1024; // 2 MB

            var uploadedUrls = new List<string>();
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request == null)
                return BadRequest<List<string>>("HTTP Context is unavailable.");

            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            string uploadPath = Path.Combine("Products", categoryName);

            foreach (var image in imageFiles)
            {
                var extension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    return BadRequest<List<string>>($"Unsupported image extension: {extension}");

                if (image.Length > maxFileSize)
                    return BadRequest<List<string>>($"Image '{image.FileName}' exceeds size limit of 2 MB.");

                var result = await _fileService.UploadImage(uploadPath, image);
                if (result.StartsWith("Error:"))
                    return BadRequest<List<string>>(result);

                uploadedUrls.Add($"{baseUrl}{result}");
            }

            return Success(uploadedUrls);
        }

        public async Task<Response<string>> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetTableNoTracking()
                        .Include(p => p.Brand)
                        .Include(p => p.Category)
                        .Include(p => p.Images)
                        .Include(p => p.Colors)
                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return BadRequest<string>("This product does not exist.");

            // Delete images from disk
            if (product.Images != null && product.Images.Any())
            {
                foreach (var image in product.Images)
                {
                    _fileService.DeleteImage(image.ImageUrl);
                }

                // Clear image records (optional if using cascade delete)
                product.Images.Clear();
            }

            // Optional: delete related product colors if needed
            if (product.Colors != null && product.Colors.Any())
            {
                product.Colors.Clear();
            }

            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.Save();

            return Deleted<string>("Product deleted successfully.");
        }

        public async Task<Response<IEnumerable<ProductDetailedDto>>> GetAllProductsDetiled()
        {
            var products = await _unitOfWork.Products.GetTableNoTracking()
                          .Include(p => p.Brand).Include(p => p.Category).Include(p => p.Images).Include(p => p.Colors)
                          .ToListAsync();

            if (products == null || products.Count == 0)
                return NotFound<IEnumerable<ProductDetailedDto>>("There are no products yet.");

            var result = products.Select(c => new ProductDetailedDto
            {
                Id = c.Id,
                Brand = c.Brand?.Name ?? "N/A", // Handle null Brand
                BrandId = c.BrandId ?? 0, // Handle null BrandId
                CategoryId = c.CategoryId ?? 0, // Handle null CategoryId
                Category = c.Category?.Name ?? "N/A", // Handle null Category
                Name = c.Name,
                Description = c.Description,
                DiscountPercentage = c.DiscountPercentage,
                Price = c.Price,
                Colors = c.Colors?.Select(color => new ColorsDto
                {
                    color = color.ColorName ?? "N/A" // Handle null ColorName
                })?.ToList() ?? new List<ColorsDto>(), // Return empty list if Colors is null
                Images = c.Images?.Select(image => new ImagesDto
                {
                    image = image.ImageUrl ?? "N/A" // Handle null ImageUrl
                })?.ToList() ?? new List<ImagesDto>(), // Return empty list if Images is null
                StockAmount = c.StockAmount,
                Rating = c.Reviews.Any() ? 1 + (c.Reviews.Average(r => r.Rate) * 4 / 100) : 1,
                TotalReviews = c.Reviews.Count(),


            });

            return Success(result);
        }
        public async Task<Response<ProductDetailedDto>> GetProductsDetailedByID(int id)
        {
            var product = await _unitOfWork.Products.GetTableNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Colors)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                var result = new ProductDetailedDto
                {
                    Id = product.Id,
                    Brand = product.Brand?.Name ?? "N/A", // Handle null Brand
                    BrandId = product.BrandId ?? 0, // Handle null BrandId
                    CategoryId = product.CategoryId ?? 0, // Handle null CategoryId
                    Category = product.Category?.Name ?? "N/A", // Handle null Category
                    Name = product.Name,
                    Description = product.Description,
                    DiscountPercentage = product.DiscountPercentage,
                    Price = product.Price,
                    Colors = product.Colors?.Select(color => new ColorsDto
                    {
                        color = color.ColorName ?? "N/A" // Handle null ColorName
                    })?.ToList() ?? new List<ColorsDto>(), // Return empty list if Colors is null
                    Images = product.Images?.Select(image => new ImagesDto
                    {
                        image = image.ImageUrl ?? "N/A" // Handle null ImageUrl
                    })?.ToList() ?? new List<ImagesDto>(), // Return empty list if Images is null
                    StockAmount = product.StockAmount,
                    Rating = product.Reviews.Any() ? 1 + (product.Reviews.Average(r => r.Rate) * 4 / 100) : 1,
                    TotalReviews = product.Reviews.Count(),
                };

                return Success(result);
            }

            return NotFound<ProductDetailedDto>("This Product is not exist.");
        }
        public async Task<Response<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _unitOfWork.Products.GetTableNoTracking()
               .Include(p => p.Brand).Include(p => p.Category).Include(p => p.Images).Include(p => p.Colors)
               .ToListAsync();

            if (products != null && products.Count() > 0)
            {
                var dto = _mapper.Map<IEnumerable<ProductDto>>(products);
                return Success(dto);
            }
            return NotFound<IEnumerable<ProductDto>>("There is no products yet.");
        }

        public async Task<Response<ProductDto>> GetProductById(int id)
        {
            var product = await _unitOfWork.Products.GetTableNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Colors)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                var dto = _mapper.Map<ProductDto>(product);
                return Success(dto);
            }

            return NotFound<ProductDto>("Product not found.");
        }
        public async Task<Response<ProductDto>> GetProductByName(string name)
        {
            var product = await _unitOfWork.Products.GetProductByName(name);
            if (product != null)
            {
                var dto = _mapper.Map<ProductDto>(product);
                return Success(dto);
            }
            return NotFound<ProductDto>("Sorry, this product does not exist!");
        }

        public async Task<Response<IEnumerable<ProductDto>>> GetProductsByBrandId(int id)
        {
            var products = await _unitOfWork.Products.GetProductsByBrandId(id);
            if (products != null)
            {
                var dto = _mapper.Map<IEnumerable<ProductDto>>(products);
                return Success(dto);
            }
            return NotFound<IEnumerable<ProductDto>>("No products in stock for this brand, it will be available soon.");
        }

        public async Task<Response<IEnumerable<ProductDto>>> GetProductsByBrandName(string name)
        {
            var products = await _unitOfWork.Products.GetProductsByBrandName(name);
            if (products != null)
            {
                var dto = _mapper.Map<IEnumerable<ProductDto>>(products);
                return Success(dto);
            }
            return NotFound<IEnumerable<ProductDto>>("No products in stock for this brand, it will be available soon.");

        }

        public async Task<Response<IEnumerable<ProductDto>>> GetProductsByCategoryId(int id)
        {
            var products = await _unitOfWork.Products.GetProductsByCategoryId(id);
            if (products != null)
            {
                var dto = _mapper.Map<IEnumerable<ProductDto>>(products);
                return Success(dto);
            }
            return NotFound<IEnumerable<ProductDto>>("No products in stock for this brand, it will be available soon.");
        }

        public async Task<Response<IEnumerable<ProductDto>>> GetProductsByCategoryName(string name)
        {
            var products = await _unitOfWork.Products.GetProductsByCategoryName(name);
            if (products != null)
            {
                var dto = _mapper.Map<IEnumerable<ProductDto>>(products);
                return Success(dto);
            }
            return NotFound<IEnumerable<ProductDto>>("No products in stock for this brand, it will be available soon.");
        }

        public async Task<Response<string>> UpdateProductAsync(int id, AddProductDto dto)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound<string>("Product not found.");

            var validCategory = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            if (validCategory == null)
                return BadRequest<string>("Invalid category ID.");

            _mapper.Map(dto, existingProduct); // Update basic fields

            // Handle image replacement
            if (dto.ImageFiles != null && dto.ImageFiles.Any())
            {
                // Validate image extensions
                if (dto.ImageFiles.Any(f => !Extentions.Contains(Path.GetExtension(f.FileName).ToLower())))
                    return BadRequest<string>("One or more image extensions are invalid.");

                var context = _httpContextAccessor.HttpContext;
                if (context?.Request == null)
                    return BadRequest<string>("HttpContext or Request is not available.");

                var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
                string path = Path.Combine("Products", validCategory.Name);

                // Delete old images from disk
                if (existingProduct.Images != null && existingProduct.Images.Any())
                {
                    foreach (var img in existingProduct.Images)
                    {
                        _fileService.DeleteImage(img.ImageUrl);
                    }

                    existingProduct.Images.Clear();
                }

                // Upload new images
                var newImageUrls = new List<string>();
                foreach (var file in dto.ImageFiles)
                {
                    var result = await _fileService.UploadImage(path, file);
                    switch (result)
                    {
                        case "NoImage":
                            return NotFound<string>("One of the images was not found.");
                        case "FailedToUploadImage":
                            return BadRequest<string>("Failed to upload an image.");
                        case "ImageSizeExceeded":
                            return BadRequest<string>("Image exceeds the 1MB size limit.");
                        case "InvalidFileType":
                            return BadRequest<string>("Invalid image file type.");
                        default:
                            newImageUrls.Add($"{baseUrl}{result}");
                            break;
                    }
                }

                // Assign new images
                existingProduct.Images = newImageUrls.Select(url => new ProductImage { ImageUrl = url }).ToList();
            }

            // Optional: Handle colors if needed
            if (dto.ColorNames != null && dto.ColorNames.Any())
            {
                existingProduct.Colors.Clear(); // Clear previous
                existingProduct.Colors = dto.ColorNames.Select(name => new ProductColor
                {
                    ColorName = name
                }).ToList();
            }

            await _unitOfWork.Save();
            return Updated<string>("Product updated successfully.");
        }


    }
}
