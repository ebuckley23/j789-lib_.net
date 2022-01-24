using J789.Library.Data.Abstraction;
using J789.Library.Data.Abstraction.Entity;
using J789.Library.Data.Abstraction.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace J789.Library.Data.EFCore
{
    public class Repository<TDbContext> : IRepository
        where TDbContext : DbContext
    {
        protected readonly DbContextBase<TDbContext> _context;
        protected readonly ILogger<Repository<TDbContext>> _logger;
        public Repository(ILogger<Repository<TDbContext>> logger, DbContextBase<TDbContext> context)
        {
            _context = context;
            _logger = logger;
        }

        #region Count
        public virtual async Task<int> CountAsync<TEntity>()
            where TEntity : class, IEntity
        {
            return await _context.Set<TEntity>().CountAsync();
        }
        public virtual async Task<int> CountAsync<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity
        {
            return await ApplySpecification(spec).CountAsync();
        }
        #endregion

        #region Read
        public virtual async Task<TEntity> GetByIdAsync<TEntity, TId>(TId id)
            where TEntity : class, IEntity
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> GetAsync<TEntity>()
            where TEntity : class, IEntity
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync();
        }

        public virtual async Task<TEntity> GetAsync<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }
        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>()
            where TEntity : class, IEntity
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public virtual async Task<IReadOnlyList<TProjection>> GetAllAsync<TEntity, TProjection>(ISpecification<TEntity> spec, IQueryProjection<TEntity, TProjection> projection)
            where TEntity : class, IEntity
        {
            return await ApplySpecification(spec)
                .Select(projection.Projection)
                .ToListAsync();
        }

        public virtual async Task<IReadOnlyList<TProjection>> GetAllAsync<TEntity, TProjection>(IQueryProjection<TEntity, TProjection> projection)
            where TEntity : class, IEntity
        {
            return await _context.Set<TEntity>()
                .Select(projection.Projection)
                .ToListAsync();
        }

        #endregion

        #region Create
        public virtual Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            return Task.FromResult(_context.Attach(entity));
        }
        #endregion

        #region Delete
        protected virtual void Remove<TEntity>(TEntity entity) 
            where TEntity : class, IEntity
        {
            _context.Remove(entity);
        }

        public virtual void Remove<TEntity>(params TEntity[] entities) 
            where TEntity : class, IEntity
        {
            foreach (var e in entities) Remove(e);
        }
        #endregion

        #region Update
        protected void Update<TEntity>(TEntity entity) 
            where TEntity : class, IEntity
        {
            _context.Update(entity);
        }

        public void Update<TEntity>(params TEntity[] entities) 
            where TEntity : class, IEntity
        {
            foreach (var e in entities) Update(e);
        }
        #endregion

        #region Private
        private IQueryable<TEntity> ApplySpecification<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity
        {
            return SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>(), spec);
        }
        #endregion
    }
}
