using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace J789.Library.Data.Abstraction.Query
{
    /// <summary>
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implemenation-entity-framework-core"/>
    /// </summary>
    public interface ISpecification<TEntity>
    {
        /// <summary>
        /// Predicate to be applied to linq query
        /// </summary>
        Expression<Func<TEntity, bool>> Criteria { get; }
        /// <summary>
        /// List of includes to be added to .Include
        /// </summary>
        List<Expression<Func<TEntity, object>>> Includes { get; }
        /// <summary>
        /// String representation of includes to be added to .Include
        /// </summary>
        List<string> IncludeStrings { get; }
        /// <summary>
        /// OrderBy to be applied to linq .OrderBy
        /// </summary>
        Expression<Func<TEntity, object>> OrderBy { get; }
        /// <summary>
        /// OrderByDescending to be applied to linq .OrderByDescending
        /// </summary>
        Expression<Func<TEntity, object>> OrderByDescending { get; }
        /// <summary>
        /// Paging information to be applied when Paging is used for the .Take
        /// </summary>
        int Take { get; }
        /// <summary>
        /// Paging information to be applied when Paging is enabled for the .Skip
        /// </summary>
        int Skip { get; }
        /// <summary>
        /// Enable paging
        /// </summary>
        bool IsPagingEnabled { get; }
        /// <summary>
        /// Enable cursor paging
        /// </summary>
        bool IsCursorPagingEnabled { get; }
    }
}
