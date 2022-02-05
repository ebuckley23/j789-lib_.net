using J789.Library.Data.Abstraction.Entity;

namespace J789.Library.Data.Query.CommonSpecs
{
    public class IsActiveSpec<TEntity>
        : Specification<TEntity> where TEntity : IEntity, ISoftDelete
    {
        /// <summary>
        /// IEntity.IsActive == true specification
        /// </summary>
        public IsActiveSpec()
            : base(x => x.IsActive)
        {
        }
    }
}
