using J789.Library.Data.Abstraction.Entity;
using System;

namespace J789.Library.Data.Query.CommonSpecs
{
    public class CursorPagingSpec<TEntity, TId> : Specification<TEntity>
        where TEntity : IEntity<TId>
        where TId : IComparable
    {
        public CursorPagingSpec(
            int numberOfItems, 
            TId cursor)
        {
            ApplyCursorPaging(numberOfItems, x => x.Id, x => IsGreaterThan(x.Id, cursor));
        }

        private bool IsGreaterThan(TId value1, TId value2)
            => value1.CompareTo(value2) > 0;
    }
}
