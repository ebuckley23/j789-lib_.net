using J789.Library.Data.Abstraction.Query;
using System;
using System.Linq.Expressions;

namespace J789.Library.Data.Query
{
    public class QueryProjection<TEntity, TProjection> : IQueryProjection<TEntity, TProjection>
    {
        public QueryProjection(Expression<Func<TEntity, TProjection>> projection)
        {
            ApplyProjection(projection);
        }
        public Expression<Func<TEntity, TProjection>> Projection { get; private set; }

        protected virtual void ApplyProjection(Expression<Func<TEntity, TProjection>> projection)
        {
            Projection = projection;
        }
    }
}
