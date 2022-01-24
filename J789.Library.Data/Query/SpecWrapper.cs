using J789.Library.Data.Abstraction.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace J789.Library.Data.Query
{
    internal sealed class SpecWrapper<TEntity> : Specification<TEntity>
    {
        public SpecWrapper(ISpecification<TEntity> spec)
            : base(spec.Criteria)
        { }
        public SpecWrapper(Expression<Func<TEntity, bool>> criteria)
            : base(criteria)
        { }

        internal void MergeInclude(IEnumerable<Expression<Func<TEntity, object>>> includes)
        {
            foreach (var include in includes)
                this.AddInclude(include);
        }

        internal void MergeInclude(IEnumerable<string> strIncludes)
        {
            foreach (var include in strIncludes)
                this.AddInclude(include);
        }
        internal void MergePaging(int skip, int take)
        {
            this.ApplyPaging(skip, take);
        }
        internal void MergeOrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            this.ApplyOrderBy(orderBy);
        }
        internal void MergeOrderByDescending(Expression<Func<TEntity, object>> orderByDescending)
        {
            this.ApplyOrderByDescending(orderByDescending);
        }
    }
}
