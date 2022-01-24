using J789.Library.Data.EFCore;
using J789.Library.Data.UnitTests.Entities;
using Microsoft.EntityFrameworkCore;

namespace J789.Library.Data.UnitTests.Fakes
{
    public class OrderContext : DbContextBase<OrderContext>
    {
        public OrderContext(DbContextOptions<OrderContext> options) 
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(x => x.Name)
                .IsRequired();

            modelBuilder.Entity<Order>()
                .HasData(Order.Missing_Name_Order, Order.Good_Order);

            modelBuilder.Entity<Ticket>()
                .HasData(Ticket.MIDDLE_TICKET, Ticket.HIGH_TICKET, Ticket.LOWEST_TICKET);
        }
    }
}
