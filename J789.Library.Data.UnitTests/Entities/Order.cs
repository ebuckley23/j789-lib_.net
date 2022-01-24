using J789.Library.Data.Abstraction.Entity;
using System;

namespace J789.Library.Data.UnitTests.Entities
{
    public class Order : IEntity<Guid>, ISoftDelete
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; private set; }

        public static Order Good_Order => new Order
        {
            Id = Guid.NewGuid(),
            Name = nameof(Good_Order),
            IsActive = true
        };

        public bool IsActiveWasSet { get; private set; }

        public static Order Missing_Name_Order = new Order
        {
            Id = Guid.NewGuid(),
            Name = nameof(Missing_Name_Order)
        };

        public void SetIsActive(bool isActive)
        {
            throw new NotImplementedException();
        }
    }
}
