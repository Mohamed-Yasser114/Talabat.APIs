using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Specifications.ProductSpecifications
{
    public class ProductSpecParams
    {
        const int maxPageSize = 10;
        private int pageSize = 5;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > maxPageSize ? maxPageSize : value; }
        }
        public int pageIndex { get; set; } = 1;
        public string? sort {  get; set; }
        public int? brandId { get; set; }
        public int? categoryId { get; set; }
        private string? search;
        public string? Search
        {
            get { return search; }
            set { search = value?.ToLower(); }
        }
    }
}
