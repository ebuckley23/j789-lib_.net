using System;
using System.Linq.Expressions;

namespace J789.Library.Data.Abstraction.Query
{
    public interface IQueryProjection<TEntity, TProjection>
    {
        Expression<Func<TEntity, TProjection>> Projection { get; }
    }
}
