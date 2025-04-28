using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.ProductDTO;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Products")]

    public class ProductController : AppControllerBase
    {
        private readonly IProductService _productService;


        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsDetiled();
            return NewResult(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductsDetailedByID(id);
            return NewResult(product);
        }

        [HttpGet("NoRating")]
        public async Task<IActionResult> GetAllProductsWithRate()
        {
            var products = await _productService.GetAllProducts();
            return NewResult(products);
        }

        [HttpGet("NoRating/{id}")]
        public async Task<IActionResult> GetProductRateById(int id)
        {
            var product = await _productService.GetProductById(id);
            return NewResult(product);
        }

        [HttpGet("Name/{name}")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            var product = await _productService.GetProductByName(name);
            return NewResult(product);
        }

        [HttpGet("ByBrandId/{id}")]
        public async Task<IActionResult> GetProductsByBrandId(int id)
        {
            var products = await _productService.GetProductsByBrandId(id);
            return NewResult(products);
        }

        [HttpGet("ByBrandName/{name}")]
        public async Task<IActionResult> GetProductsByBrandName(string name)
        {
            var products = await _productService.GetProductsByBrandName(name);
            return NewResult(products);
        }

        [HttpGet("ByCategoryId/{id}")]
        public async Task<IActionResult> GetProductsByCategoryId(int id)
        {
            var products = await _productService.GetProductsByCategoryId(id);
            return NewResult(products);
        }

        [HttpGet("ByCategoryName/{name}")]
        public async Task<IActionResult> GetProductsByCategoryName(string name)
        {
            var products = await _productService.GetProductsByCategoryName(name);
            return NewResult(products);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto dto)
        {
            var product = await _productService.AddProductAsync(dto);
            return NewResult(product);
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.DeleteProductAsync(id);
            return NewResult(product);
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] AddProductDto dto)
        {
            var response = await _productService.UpdateProductAsync(id, dto);
            return NewResult(response);
        }

    }
}
