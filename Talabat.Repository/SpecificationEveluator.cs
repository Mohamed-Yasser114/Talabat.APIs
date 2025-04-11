using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    internal static class SpecificationEveluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> queryValues, ISpecifications<T> spec)
        {
            if (spec.Criteria != null) 
                queryValues = queryValues.Where(spec.Criteria);
            if (spec.OrderBy != null)
                queryValues = queryValues.OrderBy(spec.OrderBy);
            else if(spec.OrderByDescending != null)
                queryValues = queryValues.OrderByDescending(spec.OrderByDescending);
            if(spec.IsPaginationEnabled)
                queryValues = queryValues.Skip(spec.Skip).Take(spec.Take);
            queryValues = spec.Includes.Aggregate(queryValues, (currquery, expression) => currquery.Include(expression));
            return queryValues;
        }
    }
}
