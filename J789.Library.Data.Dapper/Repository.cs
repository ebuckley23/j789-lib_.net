using Dapper;
using J789.Library.Data.Abstraction;
using J789.Library.Data.Abstraction.Entity;
using J789.Library.Data.Abstraction.Query;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace J789.Library.Data.Dapper
{
    public class Repository : IDapperRepository
    {
        protected readonly ILogger<Repository> _logger;
        protected readonly DapperContext _context;
        public Repository(ILogger<Repository> logger, DapperContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region Count
        public virtual Task<int> CountAsync<TEntity>()
            where TEntity : class, IEntity
        {
            return _context.Database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [Table]");
        }

        public virtual Task<int> CountAsync<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity
        {
            return _context.Database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [Table]");
        }
        #endregion

        #region Read
        #region Dapper Specific
        public virtual Task<TEntity> GetAsync<TEntity>(string sql, object parameters)
        {
            return _context.Database.QueryFirstOrDefaultAsync<TEntity>(
                sql,
                parameters,
                transaction: _context.CurrentTransaction);
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>(string sql, object parameters)
        {
            var result = await _context.Database.QueryAsync<TEntity>(
                sql,
                parameters, 
                transaction: _context.CurrentTransaction);

            return result.ToList();
        }
        #endregion

        public virtual Task<TEntity> GetByIdAsync<TEntity, TId>(TId id)
            where TEntity : class, IEntity
        {
            return _context.Database.QuerySingleAsync<TEntity>(
                "SELECT TOP 1 * FROM [table] WHERE Id = @id", 
                new { Id = id },
                transaction: _context.CurrentTransaction);
        }

        public virtual Task<TEntity> GetAsync<TEntity>()
            where TEntity: class, IEntity
        {
            return _context.Database.QueryFirstOrDefaultAsync<TEntity>(
                "SELECT TOP 1 * FROM [table]",
                transaction: _context.CurrentTransaction);
        }

        public virtual Task<TEntity> GetAsync<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity
        {
            var where = new DapperQueryTranslator().Translate(spec.Criteria);
            return null;
        }

        Task<IReadOnlyList<TEntity>> IRepository.GetAllAsync<TEntity>()
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<TEntity>> IRepository.GetAllAsync<TEntity>(ISpecification<TEntity> spec)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<TProjection>> IRepository.GetAllAsync<TEntity, TProjection>(ISpecification<TEntity> spec, IQueryProjection<TEntity, TProjection> projection)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<TProjection>> IRepository.GetAllAsync<TEntity, TProjection>(IQueryProjection<TEntity, TProjection> projection)
        {
            throw new NotImplementedException();
        }

        Task IRepository.AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void IRepository.Remove<TEntity>(params TEntity[] entities)
        {
            throw new NotImplementedException();
        }

        void IRepository.Update<TEntity>(params TEntity[] entities)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public interface IDapperRepository : IRepository
    {
        Task<TEntity> GetAsync<TEntity>(string sql, object parameters);
    }

    public class DapperQueryTranslator : ExpressionVisitor
    {
        private StringBuilder sb;
        private string _orderBy = string.Empty;
        private int? _skip = null;
        private int? _take = null;
        private string _where = string.Empty;

        public int? Skip => _skip;
        public int? Take => _take;
        public string OrderBy => _orderBy;
        public string Where => _where;

        public DapperQueryTranslator()
        {

        }

        public string Translate(Expression expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);
            _where = this.sb.ToString();
            return _where;
        }

        private static Expression StripQuotes(Expression e)
        {
            while(e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if(node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
            {
                this.Visit(node.Arguments[0]);
                var lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                this.Visit(lambda.Body);
                return node;
            }
            else if (node.Method.Name == "Take")
            {
                if (this.ParseTakeExpression(node))
                {
                    var nextExpression = node.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (node.Method.Name == "Skip")
            {
                if (this.ParseSkipExpression(node))
                {
                    var nextExpression = node.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (node.Method.Name == "OrderBy")
            {
                if (this.ParseOrderByExpression(node, "ASC"))
                {
                    var nextExpression = node.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (node.Method.Name == "OrderByDescending")
            {
                if (this.ParseOrderByExpression(node, "DESC"))
                {
                    var nextExpression = node.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            throw new NotSupportedException($"The method {node.Method.Name} is not supported.");
            //return base.VisitMethodCall(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    this.Visit(node.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(node.Operand);
                    break;
                default:
                    throw new NotSupportedException($"The unary operator {node.NodeType} is not supported");
            }

            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            sb.Append("(");
            this.Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;
                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    sb.Append(" OR ");
                    break;
                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    if (IsNullConstant(node.Right))
                        sb.Append(" IS ");
                    else 
                        sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    if (IsNullConstant(node.Right))
                        sb.Append(" IS NOT ");
                    else
                        sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;
                default:
                    throw new Exception($"The binary operator {node.NodeType} is not supported.");
            };

            this.Visit(node.Right);
            sb.Append(")");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            IQueryable q = node.Value as IQueryable;

            if (q == null && node.Value == null) sb.Append("NULL");
            else if (q == null)
            {
                switch (Type.GetTypeCode(node.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)node.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(node.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.DateTime:
                        sb.Append("'");
                        sb.Append(node.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException($"The constant for {node.Value} is not supported.");
                    default:
                        sb.Append(node.Value);
                        break;
                }
            }
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && (node.Expression.NodeType == ExpressionType.Parameter || node.Expression.NodeType == ExpressionType.Constant))
            {
                sb.Append(node.Member.Name);
                return node;
            }
            throw new NotSupportedException($"The member {node.Member.Name} is not supported.");
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            sb.Append($"@{node.Name}");
            return node;
            //return base.VisitParameter(node);
        }

        protected bool IsNullConstant(Expression exp)
            => (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);

        private bool ParseOrderByExpression(MethodCallExpression expression, string orderBy)
        {
            var unary = (UnaryExpression)expression.Arguments[1];
            var lambdaExpression = (LambdaExpression)unary.Operand;
            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            var body = lambdaExpression.Body as MemberExpression;
            if (body == null) return false;

            if (string.IsNullOrEmpty(_orderBy))
            {
                _orderBy = $"{body.Member.Name} {orderBy}";
            }
            else
            {
                _orderBy = $"{_orderBy} {body.Member.Name} {orderBy}";
            }
            return true;
        }

        private bool ParseTakeExpression(MethodCallExpression expression)
        {
            var sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if(int.TryParse(sizeExpression.Value.ToString(), out size))
            {
                _take = size;
                return true;
            }

            return false;
        }

        private bool ParseSkipExpression(MethodCallExpression expression)
        {
            var sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if (int.TryParse(sizeExpression.Value.ToString(), out size))
            {
                _take = size;
                return true;
            }

            return false;
        }
    }

    public static class Evaluator
    {
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        class SubtreeEvaluator : ExpressionVisitor
        {
            HashSet<Expression> candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }

            [return: NotNullIfNotNull("node")]
            public override Expression Visit(Expression node)
            {
                if (node == null) return null;
                if (this.candidates.Contains(node)) return this.Evaluate(node);
                return base.Visit(node);
            }

            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant) return e;

                var lambda = Expression.Lambda(e);
                Delegate fn = lambda.Compile();
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }

        class Nominator : ExpressionVisitor
        {
            Func<Expression, bool> fnCanBeEvaluated;
            HashSet<Expression> candidates;
            bool cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                this.candidates = new HashSet<Expression>();
                this.Visit(expression);
                return this.candidates;
            }

            [return: NotNullIfNotNull("node")]
            public override Expression Visit(Expression node)
            {
                if (node == null) return node;

                bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                this.cannotBeEvaluated = false;
                base.Visit(node);
                if (!this.cannotBeEvaluated)
                {
                    if (this.fnCanBeEvaluated(node))
                    {
                        this.candidates.Add(node);
                    }
                    else
                    {
                        this.cannotBeEvaluated = true;
                    }
                }
                this.cannotBeEvaluated |= saveCannotBeEvaluated;
                return node;
            }
        }
    }
}
