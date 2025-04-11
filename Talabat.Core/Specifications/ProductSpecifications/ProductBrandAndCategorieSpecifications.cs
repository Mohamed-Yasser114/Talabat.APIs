using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpecifications
{
    public class ProductBrandAndCategorieSpecifications : BaseSpecifications<Product>
    {
        public ProductBrandAndCategorieSpecifications(ProductSpecParams spec) : base(
            P => (string.IsNullOrEmpty(spec.Search) || P.Name.ToLower().Contains(spec.Search)) &&
                 (!spec.brandId.HasValue || P.BrandId == spec.brandId.Value) &&
                 (!spec.categoryId.HasValue || P.CategoryId == spec.categoryId.Value)
            )
        {
            Include();

            if (!string.IsNullOrEmpty(spec.sort))
            {
                switch (spec.sort)
                {
                    case "priceAsc":
                        OrderBy = P => P.Price;
                        break;
                    case "priceDesc":
                        OrderByDescending = P => P.Price;
                        break;
                    default: 
                        OrderBy = P => P.Name; 
                        break;
                }
            }
            else
                OrderBy = P => P.Name;

            ApplyPagenation((spec.pageIndex - 1) * spec.PageSize, spec.PageSize);

        }
        public ProductBrandAndCategorieSpecifications(int id) : base(P => P.Id == id)
        {
            Include();
        }
        private void Include()
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
        }
    }
}
