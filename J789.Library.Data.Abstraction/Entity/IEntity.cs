namespace J789.Library.Data.Abstraction.Entity
{
    public interface IEntity
    {
    }

    public interface IEntity<TId> : IEntity
    {
        TId Id { get; }
    }
}
