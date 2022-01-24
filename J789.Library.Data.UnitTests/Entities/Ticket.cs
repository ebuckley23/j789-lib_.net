using J789.Library.Data.Abstraction.Entity;

namespace J789.Library.Data.UnitTests.Entities
{
    public class Ticket : IEntity<int>, ISoftDelete
    {
        public int Id { get; set; }
        public int TicketNumber { get; set; }

        public bool IsActiveWasSet { get; private set; }

        public bool IsActive { get; private set; }

        public void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }

        public static Ticket LOWEST_TICKET => new Ticket { Id = 123, TicketNumber = 26, IsActive = true };
        public static Ticket MIDDLE_TICKET => new Ticket { Id = 124, TicketNumber = 62, IsActive = false };
        public static Ticket HIGH_TICKET => new Ticket { Id = 125, TicketNumber = 97, IsActive = true };
    }
}
