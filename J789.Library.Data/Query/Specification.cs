using J789.Library.Data.Abstraction.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace J789.Library.Data.Query
{
    public abstract class Specification<TEntity> : ISpecification<TEntity>
    {
        protected Specification()
            : this(null)
        { }

        protected Specification(Expression<Func<TEntity, bool>> criteria)
        {
            ApplyCriteria(criteria);
        }

        public Expression<Func<TEntity, bool>> Criteria { get; private set; }

        public List<Expression<Func<TEntity, object>>> Includes { get; }
            = new List<Expression<Func<TEntity, object>>>();

        public List<string> IncludeStrings { get; } = new List<string>();

        public Expression<Func<TEntity, object>> OrderBy { get; private set; }

        public Expression<Func<TEntity, object>> OrderByDescending { get; private set; }

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnabled { get; private set; }

        public bool IsCursorPagingEnabled { get; private set; }

        protected virtual void AddInclude(Expression<Func<TEntity, object>> include)
        {
            Includes.Add(include);
        }

        protected virtual void AddInclude(string include)
        {
            IncludeStrings.Add(include);
        }
        protected virtual void ApplyPaging(int skip, int take)
        {
            if (IsCursorPagingEnabled) throw new InvalidOperationException("Cursor paging is already enabled.");

            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyCursorPaging(
            int take, 
            Expression<Func<TEntity, object>> orderBy,
            Expression<Func<TEntity, bool>> criteria)
        {
            if (IsPagingEnabled) throw new InvalidOperationException("Paging is already enabled.");

            ApplyOrderBy(orderBy);
            ApplyCriteria(criteria);
            Take = take;
            IsCursorPagingEnabled = true;
        }
        protected virtual void ApplyOrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            OrderBy = orderBy;
        }
        protected virtual void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescending)
        {
            OrderByDescending = orderByDescending;
        }

        protected virtual void ApplyCriteria(Expression<Func<TEntity, bool>> criteria)
        {
            if (Criteria != null) throw new InvalidOperationException("Criteria has already been set.");
            Criteria = criteria;
        }
    }
}
