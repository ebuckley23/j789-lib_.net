namespace J789.Library.Data.Abstraction.Entity
{
    public interface ISoftDelete
    {
        bool IsActiveWasSet { get; }
        bool IsActive { get; }
        void SetIsActive(bool isActive);
    }
}
