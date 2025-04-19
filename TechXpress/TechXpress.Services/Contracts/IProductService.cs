using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Services.DTOs.ProductDtos;

namespace TechXpress.Services.Contracts
{
    public interface IProductService
    {
        ProductListDto GetProduct(int id);

        IEnumerable<ProductListDto> GetProductList();

        bool InsertProduct(ProductInsertDto product);

        bool UpdateProduct(ProductUpdateDto product);

        bool DeleteProduct(int id);
    }
}
