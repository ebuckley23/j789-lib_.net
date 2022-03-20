using J789.Library.Data.Abstraction.Query;
using System;
using System.Linq.Expressions;

namespace J789.Library.Data.Query.CommonSpecs
{
    public class PaginationSpec<TEntity> : Specification<TEntity>
    {
        /// <summary>
        /// Pagination specification for paging operations.
        /// </summary>
        /// <param name="pageNumber">Page to start with</param>
        /// <param name="numberOfItems">Number of items to return</param>
        /// <param name="criteria">Criteria to apply to dataset. This will be applied before the page.</param>
        /// <exception cref="ArgumentException"></exception>
        public PaginationSpec(int pageNumber, int numberOfItems, Expression<Func<TEntity, bool>> criteria)
            : base(criteria)
        {
            if (pageNumber < 1) throw new ArgumentException("pageNumber can not be less than 1");
            ApplyPaging(numberOfItems * (pageNumber - 1), numberOfItems);
        }

        /// <summary>
        /// Pagination specification for paging operations.
        /// </summary>
        /// <param name="pageNumber">Page to start with</param>
        /// <param name="numberOfItems">Number of items to return</param>
        public PaginationSpec(int pageNumber, int numberOfItems)
            : this(pageNumber, numberOfItems, x => true)
        { }

        /// <summary>
        /// Pagination specification for paging operations
        /// </summary>
        /// <param name="pageNumber">Page to start with</param>
        /// <param name="numberOfItems">Number of items to return</param>
        /// <param name="spec">Additional specification to compose.</param>
        public PaginationSpec(int pageNumber, int numberOfItems, ISpecification<TEntity> spec)
            : this(pageNumber, numberOfItems, spec.Criteria)
        { }
    }
}
