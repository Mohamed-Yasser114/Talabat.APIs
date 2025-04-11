using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications.ProductSpecifications;

namespace Talabat.Core.IServices
{
    public interface IProductService
    {
        Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams spec);
        Task<int> GetCountAsync(ProductSpecParams spec);
        Task<Product?> GetProductByIdAsync(int id);
        Task<IReadOnlyList<ProductBrand>> GetBrandsAsync();
        Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync();
    }
}
