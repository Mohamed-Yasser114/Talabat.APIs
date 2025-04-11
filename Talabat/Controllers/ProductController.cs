using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;
using Talabat.Core.IServices;
using Talabat.Core.Specifications.ProductSpecifications;

namespace Talabat.APIs.Controllers
{
    public class ProductController : BaseAPIController
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService,
            IMapper mapper) {
            _productService = productService;
            _mapper = mapper;
        }
        [Cached(600)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams spec)
        {

            var products = await _productService.GetProductsAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var productsCountSpec = new ProductWithFilterationForCountSpecification(spec);

            var count = await _productService.GetCountAsync(spec);

            return Ok(new Pagination<ProductToReturnDto>(spec.PageSize, spec.pageIndex, count, data));
        }
        [Cached(600)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
                return NotFound(new APIsResponse(404));
            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }
        [Cached(600)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _productService.GetBrandsAsync();
            return Ok(brands);
        }
        [Cached(600)]
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}
