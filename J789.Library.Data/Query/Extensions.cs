using J789.Library.Data.Abstraction.Query;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace J789.Library.Data.Query
{
    public static class Extensions
    {
        public static ISpecification<TEntity> And<TEntity>(this ISpecification<TEntity> spec1, ISpecification<TEntity> spec2)
        {
            Expression<Func<TEntity, bool>> leftExpression = spec1.Criteria;
            Expression<Func<TEntity, bool>> rightExpression = spec2.Criteria;

            var paramExpr = Expression.Parameter(typeof(TEntity));
            var exprBody = Expression.AndAlso(leftExpression.Body, rightExpression.Body);
            exprBody = (BinaryExpression)new ParameterExpressionVisitor(paramExpr).Visit(exprBody);

            var lambda = Expression.Lambda<Func<TEntity, bool>>(exprBody, paramExpr);
            return Merge(spec1, spec2, lambda); // new OperatorExpressionSpec<TEntity>(lambda);
        }

        public static ISpecification<TEntity> Or<TEntity>(this ISpecification<TEntity> spec1, ISpecification<TEntity> spec2)
        {
            Expression<Func<TEntity, bool>> leftExpression = spec1.Criteria;
            Expression<Func<TEntity, bool>> rightExpression = spec2.Criteria;
            var paramExpr = Expression.Parameter(typeof(TEntity));
            var exprBody = Expression.OrElse(leftExpression.Body, rightExpression.Body);
            exprBody = (BinaryExpression)new ParameterExpressionVisitor(paramExpr).Visit(exprBody);

            var lambda = Expression.Lambda<Func<TEntity, bool>>(exprBody, paramExpr);
            return Merge(spec1, spec2, lambda); // new OperatorExpressionSpec<TEntity>(lambda);
        }

        public static ISpecification<TEntity> Not<TEntity>(this ISpecification<TEntity> spec1)
        {
            Expression<Func<TEntity, bool>> expression = spec1.Criteria;
            var paramExpr = Expression.Parameter(typeof(TEntity));
            var exprBody = Expression.Not(expression);
            exprBody = (UnaryExpression)new ParameterExpressionVisitor(paramExpr).Visit(exprBody);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(exprBody, paramExpr);
            return new SpecWrapper<TEntity>(lambda);
        }

        /// <summary>
        /// On merge, always take the right most paging and project attributes
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private static ISpecification<TEntity> Merge<TEntity>(ISpecification<TEntity> left, ISpecification<TEntity> right, Expression<Func<TEntity, bool>> criteria)
        {
            var ret = new SpecWrapper<TEntity>(criteria);

            ret.MergeInclude(left.IncludeStrings.Concat(right.IncludeStrings));
            ret.MergeInclude(left.Includes.Concat(right.Includes));

            if (right.IsPagingEnabled)
            {
                ret.MergePaging(right.Skip, right.Take);
            }
            if (right.OrderBy != null)
            {
                ret.MergeOrderBy(right.OrderBy);
            }
            if (right.OrderByDescending != null)
            {
                ret.MergeOrderByDescending(right.OrderByDescending);
            }

            return ret;
        }
    }

    public class ParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        public ParameterExpressionVisitor(ParameterExpression parameter)
        {
            _parameter = parameter;
        }
        protected override Expression VisitParameter(ParameterExpression node)
            => base.VisitParameter(_parameter);

    }
}
