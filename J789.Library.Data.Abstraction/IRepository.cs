using J789.Library.Data.Abstraction.Entity;
using J789.Library.Data.Abstraction.Query;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace J789.Library.Data.Abstraction
{
    public interface IRepository
    {
        /// <summary>
        /// Get count of all entities
        /// </summary>
        /// <typeparam name="TEntity">Entity type to query</typeparam>
        /// <returns>Count of all entities</returns>
        Task<int> CountAsync<TEntity>() where TEntity : class, IEntity;

        /// <summary>
        /// Get count of all entities by specification
        /// </summary>
        /// <typeparam name="TEntity">Entity type to query</typeparam>
        /// <param name="spec">ISpecificationt to use</param>
        /// <returns>Count of all entities potentially filtered by specification</returns>
        Task<int> CountAsync<TEntity>(ISpecification<TEntity> spec) where TEntity : class, IEntity;

        /// <summary>
        /// Get entity by Id
        /// </summary>
        /// <typeparam name="TEntity">Entity type to query</typeparam>
        /// <typeparam name="TId">Entity Id type</typeparam>
        /// <param name="id">Entity Id</param>
        /// <returns>Entity with specified Id</returns>
        Task<TEntity> GetByIdAsync<TEntity, TId>(TId id)
            where TEntity : class, IEntity;

        /// <summary>
        /// Get entity of specified entity type
        /// </summary>
        /// <typeparam name="TEntity">Entity type to query</typeparam>
        /// <returns>The first of the Entity type found or null</returns>
        Task<TEntity> GetAsync<TEntity>()
            where TEntity : class, IEntity;

        /// <summary>
        /// Get entity of specified entity type
        /// </summary>
        /// <typeparam name="TEntity">Entity type to query</typeparam>
        /// <param name="spec">ISpecification to use</param>
        /// <returns>The first of the Entity type found or null</returns>
        Task<TEntity> GetAsync<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity;

        /// <summary>
        /// Get all entities of specified entity type
        /// </summary>
        /// <typeparam name="TEntity">Entity type to query</typeparam>
        /// <returns>All entities of the specified type</returns>
        Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>()
            where TEntity : class, IEntity;

        /// <summary>
        /// Get all entities of the specified entity type based on the specification criteria
        /// </summary>
        /// <typeparam name="TEntity">Entity type to query</typeparam>
        /// <param name="spec">ISpecification to use</param>
        /// <returns>IReadOnlyList of the specified entity type</returns>
        Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity;

        /// <summary>
        /// Get all entities of the specified entity type based on the specification criteria
        /// </summary>
        /// <typeparam name="TEntity">Entity type to query</typeparam>
        /// <typeparam name="TProjection">Object to project entity types to</typeparam>
        /// <param name="spec">ISpecification to use</param>
        /// <param name="projection">IQueryProjection to project entities to</param>
        /// <returns>IReadOnlyList of projected entities of the specified entity type</returns>
        Task<IReadOnlyList<TProjection>> GetAllAsync<TEntity, TProjection>(ISpecification<TEntity> spec, IQueryProjection<TEntity, TProjection> projection)
            where TEntity : class, IEntity;

        /// <summary>
        /// Get all entities of the specified entity type
        /// </summary>
        /// <typeparam name="TEntity">Entity type to query</typeparam>
        /// <typeparam name="TProjection">Object to project entity types to</typeparam>
        /// <param name="projection">IQueryProjection to project entities to</param>
        /// <returns>IReadOnlyList of projected entities of the specified entity type</returns>
        Task<IReadOnlyList<TProjection>> GetAllAsync<TEntity, TProjection>(IQueryProjection<TEntity, TProjection> projection)
            where TEntity : class, IEntity;

        /// <summary>
        /// Attach entity to be added
        /// </summary>
        /// <typeparam name="TEntity">Entity type to add</typeparam>
        /// <param name="entity">Entity to add</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task result</returns>
        Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity;

        /// <summary>
        /// Attach entities to be removed
        /// </summary>
        /// <typeparam name="TEntity">Entity type to remove</typeparam>
        /// <param name="entities">Entities to remove</param>
        void Remove<TEntity>(params TEntity[] entities)
            where TEntity : class, IEntity;

        /// <summary>
        /// Attach entities to be updated
        /// </summary>
        /// <typeparam name="TEntity">Entity type to update</typeparam>
        /// <param name="entities">Entities to be updated</param>
        void Update<TEntity>(params TEntity[] entities) 
            where TEntity : class, IEntity;
    }
}
