using System;
using System.Linq.Expressions;

namespace J789.Library.Data.Query.CommonSpecs
{
    public class OrderBySpec<TEntity> : Specification<TEntity>
    {
        private OrderBySpec(OrderByDir orderByDir, Expression<Func<TEntity, object>> orderBy)
            : base(x => true)
        {
            if (orderByDir == OrderByDir.ASC) ApplyOrderBy(orderBy);
            else ApplyOrderByDescending(orderBy);
        }

        /// <summary>
        /// Order by ascending specification
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static OrderBySpec<TEntity> Ascending(Expression<Func<TEntity, object>> orderBy)
            => new OrderBySpec<TEntity>(OrderByDir.ASC, orderBy);

        /// <summary>
        /// Order by descending specification
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static OrderBySpec<TEntity> Descending(Expression<Func<TEntity, object>> orderBy)
            => new OrderBySpec<TEntity>(OrderByDir.DESC, orderBy);
    }

    public enum OrderByDir
    {
        ASC,
        DESC
    }
}
