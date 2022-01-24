using J789.Library.Data.Abstraction.Entity;

namespace J789.Library.Data.UnitTests.Entities
{
    public class User : IEntity<int>, ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static User Good_User => new User
        {
            Id = 99,
            Name = nameof(Good_User),
            IsActive = true
        };

        public bool IsActiveWasSet { get; private set;  }

        public bool IsActive { get; private set; }

        public static User Missing_Name_User = new User
        {
            Id = 101,
            Name = nameof(Missing_Name_User)
        };

        public void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
