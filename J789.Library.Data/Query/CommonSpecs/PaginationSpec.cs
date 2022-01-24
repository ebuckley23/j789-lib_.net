using J789.Library.Data.Abstraction.Query;
using System;
using System.Linq.Expressions;

namespace J789.Library.Data.Query.CommonSpecs
{
    public class PaginationSpec<TEntity> : Specification<TEntity>
    {
        public PaginationSpec(int pageNumber, int numberOfItems, Expression<Func<TEntity, bool>> criteria)
            : base(criteria)
        {
            if (pageNumber < 1) throw new ArgumentException("pageNumber can not be less than 1");
            ApplyPaging(numberOfItems * (pageNumber - 1), numberOfItems);
        }

        public PaginationSpec(int pageNumber, int numberOfItems)
            : this(pageNumber, numberOfItems, x => true)
        { }

        public PaginationSpec(int pageNumber, int numberOfItems, ISpecification<TEntity> spec)
            : this(pageNumber, numberOfItems, spec.Criteria)
        { }
    }
}
