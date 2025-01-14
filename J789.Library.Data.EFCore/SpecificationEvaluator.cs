﻿using J789.Library.Data.Abstraction.Entity;
using J789.Library.Data.Abstraction.Query;
using J789.Library.Data.EFCore.Abstraction.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace J789.Library.Data.EFCore
{
    internal class SpecificationEvaluator
    {
        public static IQueryable<TEntity> GetQuery<TEntity>(IQueryable<TEntity> query, ISpecification<TEntity> specification)
            where TEntity : class, IEntity
        {
            var ret = query;
            if (specification.Criteria != null)
            {
                ret = ret.Where(specification.Criteria);
            }

            // Includes all expression-based includes
            ret = specification.Includes.Aggregate(ret,
                (current, include) => current.Include(include));

            // Include any string-based include statements
            ret = specification.IncludeStrings.Aggregate(ret,
                (current, include) => current.Include(include));

            if (specification.OrderBy != null)
            {
                ret = ret.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                ret = ret.OrderByDescending(specification.OrderByDescending);
            }

            // Apply paging if enabled
            if (specification.IsPagingEnabled)
            {
                ret = ret.Skip(specification.Skip).Take(specification.Take);
            }

            if (specification.IsCursorPagingEnabled)
            {
                ret = ret.OrderBy(specification.OrderBy)
                    .Take(specification.Take);
            }

            return ret.TagWithSource();
        }
    }
}
