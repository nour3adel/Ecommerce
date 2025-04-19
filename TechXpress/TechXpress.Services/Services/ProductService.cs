using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.Contracts;
using TechXpress.Services.Contracts;
using TechXpress.Services.DTOs.ProductDtos;

namespace TechXpress.Services.Services
{
    internal class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        private IProductRepository _products;
        public ProductService(IUnitOfWork unitOfWork) { 
            _unitOfWork = unitOfWork;
            _products = _unitOfWork.Products;
        
        }
        public bool DeleteProduct(int id)
        {
            throw new NotImplementedException();
        }

        public ProductListDto GetProduct(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductListDto> GetProductList()
        {
            throw new NotImplementedException();
        }

        public bool InsertProduct(ProductInsertDto product)
        {
            throw new NotImplementedException();
        }

        public bool UpdateProduct(ProductUpdateDto product)
        {
            throw new NotImplementedException();
        }
    }
}
