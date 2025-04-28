using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.BrandDTO;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Brands")]

    public class BrandController : AppControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet("BrandId/{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            if (ModelState.IsValid)
            {
                var response = await _brandService.GetBrandByID(id);
                return NewResult(response);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("BrandName/{name}")]
        public async Task<IActionResult> GetBrandByName(string name)
        {
            if (ModelState.IsValid)
            {
                var response = await _brandService.GetBrandByName(name);
                return NewResult(response);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("Brands")]
        public async Task<IActionResult> GetAllBrands()
        {
            if (ModelState.IsValid)
            {
                var response = await _brandService.GetAllBrands();
                return NewResult(response);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("AddBrand")]
        public async Task<IActionResult> AddBrand(AddBrandDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _brandService.AddBrand(dto);
                return NewResult(response);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("UpdateBrand")]
        public async Task<IActionResult> EditBrand(int id, BrandDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _brandService.UpdateBrand(id, dto);
                return NewResult(response);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteBrand")]
        public async Task<IActionResult> RemoveBrand(int id)
        {
            if (ModelState.IsValid)
            {
                var response = await _brandService.DeleteBrand(id);
                return NewResult(response);
            }
            return BadRequest(ModelState);
        }
    }
}
