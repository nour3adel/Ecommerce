using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.BrandDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class BrandService : ResponseHandler, IBrandService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Response<string>> AddBrand(AddBrandDto dto)
        {
            var brand = _mapper.Map<Brand>(dto);

            await _unitOfWork.Brands.AddAsync(brand);
            await _unitOfWork.Save();
            return Success("Brand Added Successfully");
        }

        public async Task<Response<string>> DeleteBrand(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound<string>("Brand Not Found");

            }

            var dto = _mapper.Map<BrandDto>(brand);
            await _unitOfWork.Brands.DeleteAsync(brand);
            await _unitOfWork.Save();

            return Deleted<string>("Brand Deleted Successfully");

        }

        public async Task<Response<List<BrandDto>>> GetAllBrands()
        {
            var brands = await _unitOfWork.Brands.GetTableNoTracking().ToListAsync();

            if (brands != null && brands.Count() > 0)
            {
                var dto = _mapper.Map<List<BrandDto>>(brands);
                return Success(dto);

            }
            return NotFound<List<BrandDto>>("Brand Not Found");

        }

        public async Task<Response<BrandDto>> GetBrandByID(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand != null)
            {
                var dto = _mapper.Map<BrandDto>(brand);
                return Success(dto);

            }
            return NotFound<BrandDto>("Brand Not Found");
        }

        public async Task<Response<BrandDto>> GetBrandByName(string name)
        {
            var brand = await _unitOfWork.Brands.GetBrandByName(name);
            if (brand != null)
            {
                var dto = _mapper.Map<BrandDto>(brand);
                return Success(dto);


            }
            return NotFound<BrandDto>("Brand Not Found");

        }

        public async Task<Response<string>> UpdateBrand(int id, BrandDto dto)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound<string>("Brand Not Found");
            }

            var newBrand = _mapper.Map<Brand>(dto);
            newBrand.Id = id;
            newBrand.Products = brand.Products;
            newBrand.Email = brand.Email;
            newBrand.Phone = brand.Phone;
            newBrand.Name = brand.Name;

            await _unitOfWork.Brands.UpdateAsync(newBrand);
            await _unitOfWork.Save();

            return Updated<string>("Brand Updated Successfully");
        }

    }
}

