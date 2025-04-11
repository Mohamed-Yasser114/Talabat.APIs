using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.IServices;
using Talabat.Core.Specifications.ProductSpecifications;

namespace Talabat.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
        {
            var brands = await _unitOfWork.repository<ProductBrand>().GetAllAsync();

            return brands;
        }

        public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
        {
            var categories = await _unitOfWork.repository<ProductCategory>().GetAllAsync();

            return categories;
        }

        public async Task<int> GetCountAsync(ProductSpecParams spec)
        {
            var productsCountSpec = new ProductWithFilterationForCountSpecification(spec);

            var count = await _unitOfWork.repository<Product>().CountAsync(productsCountSpec);

            return count;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var spec = new ProductBrandAndCategorieSpecifications(id);

            var product = await _unitOfWork.repository<Product>().GetWithSpecAsync(spec);

            return product;
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams spec)
        {
            var productSpec = new ProductBrandAndCategorieSpecifications(spec);

            var products = await _unitOfWork.repository<Product>().GetAllWithSpecAsync(productSpec);

            return products;
        }
    }
}
