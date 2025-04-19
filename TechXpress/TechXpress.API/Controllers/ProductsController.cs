using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Services.Contracts;
using TechXpress.Services.DTOs.ProductDtos;

namespace TechXpress.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetAll() { 
            return Ok(_productService.GetProductList());
        
        }

        [HttpPost]
        public IActionResult Post(ProductInsertDto product) {
            return Ok(_productService.InsertProduct(product));
        }
    }
}
