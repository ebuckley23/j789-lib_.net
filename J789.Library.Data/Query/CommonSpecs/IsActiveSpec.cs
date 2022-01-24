using J789.Library.Data.Abstraction.Entity;

namespace J789.Library.Data.Query.CommonSpecs
{
    public class IsActiveSpec<TEntity>
        : Specification<TEntity> where TEntity : IEntity, ISoftDelete
    {
        public IsActiveSpec()
            : base(x => x.IsActive)
        {
        }
    }
}
