
using E_CommerceAPI.ENTITES.DTOs.BrandDTO;
using E_CommerceAPI.SERVICES.Bases;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IBrandService
    {
        public Task<Response<List<BrandDto>>> GetAllBrands();

        public Task<Response<BrandDto>> GetBrandByID(int id);
        public Task<Response<BrandDto>> GetBrandByName(string name);

        public Task<Response<string>> AddBrand(AddBrandDto brand);
        public Task<Response<string>> UpdateBrand(int id, BrandDto name);
        public Task<Response<string>> DeleteBrand(int id);

    }
}
