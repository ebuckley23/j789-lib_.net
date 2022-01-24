using J789.Library.Data.EFCore;
using J789.Library.Data.UnitTests.Entities;
using Microsoft.EntityFrameworkCore;

namespace J789.Library.Data.UnitTests.Fakes
{
    public class UserContext : DbContextBase<UserContext>
    {
        public UserContext(DbContextOptions<UserContext> options) 
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(x => x.Name)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasData(User.Missing_Name_User, User.Good_User);
        }
    }
}
