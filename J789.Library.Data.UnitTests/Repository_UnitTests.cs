using J789.Library.Data.EFCore;
using J789.Library.Data.Query;
using J789.Library.Data.Query.CommonSpecs;
using J789.Library.Data.UnitTests.Entities;
using J789.Library.Data.UnitTests.Entities.Projections;
using J789.Library.Data.UnitTests.Fakes;
using J789.Library.Data.UnitTests.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace J789.Library.Data.UnitTests
{
    public class Repository_UnitTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _dbFixture;
        private readonly Mock<ILogger<Repository<UserContext>>> uCXTLogger = new Mock<ILogger<Repository<UserContext>>>();
        private readonly Mock<ILogger<Repository<OrderContext>>> oCXTLogger = new Mock<ILogger<Repository<OrderContext>>>();
        public Repository_UnitTests(DatabaseFixture databaseFixture)
        {
            _dbFixture = databaseFixture;
        }

        #region Count Tests
        [Fact]
        public async Task Can_Get_Count()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);

            var count = await repository.CountAsync<User>();
            Assert.True(count > 0);
        }

        [Fact]
        public async Task Can_Get_Count_By_Specification()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var countOfActiveUsers = userContext.Users.Where(x => x.IsActive).Count();

            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);
            var count = await repository.CountAsync(new IsActiveSpec<User>());

            Assert.Equal(countOfActiveUsers, count);
        }
        #endregion

        #region Read Tests
        [Fact]
        public async Task Can_Get_User_By_Id()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);

            var user = await repository.GetByIdAsync<User, int>(User.Good_User.Id);
            Assert.NotNull(user);
            Assert.Equal(User.Good_User.Id, user.Id);
        }

        [Fact]
        public async Task Can_Get_First_User()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);

            var user = await repository.GetAsync<User>();
            Assert.NotNull(user);

        }

        [Fact]
        public async Task Can_Get_First_User_By_Specification()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);

            var activeUser = await repository.GetAsync(new IsActiveSpec<User>());
            Assert.NotNull(activeUser);
            Assert.True(activeUser.IsActive);
        }

        [Fact]
        public async Task Can_Get_All_Users()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);
            var count = userContext.Users.Count();

            var allUsers = await repository.GetAllAsync<User>();

            Assert.NotEmpty(allUsers);
            Assert.Equal(count, allUsers.Count);
        }

        [Fact]
        public async Task Can_Get_All_Active_Users()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);
            var count = userContext.Users.Where(x => x.IsActive).Count();

            var allUsers = await repository.GetAllAsync(new IsActiveSpec<User>());

            Assert.NotEmpty(allUsers);
            Assert.Equal(count, allUsers.Count);
            Assert.All(allUsers, _ =>
            {
                Assert.True(_.IsActive);
            });
        }

        [Fact]
        public async Task Can_Get_All_Active_User_As_UserProjection()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);
            var count = userContext.Users.Where(x => x.IsActive).Count();

            var allUsers = await repository.GetAllAsync(
                new IsActiveSpec<User>(), 
                new QueryProjection<User, UserProjection>(x => new UserProjection { NewName = x.Name, Active = x.IsActive }));

            Assert.NotEmpty(allUsers);
            Assert.Equal(count, allUsers.Count);
            Assert.All(allUsers, _ =>
            {
                Assert.True(_.Active);
            });
        }

        [Fact]
        public async Task Can_Get_All_Active_User_As_DynamicProjection()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);
            var count = userContext.Users.Where(x => x.IsActive).Count();

            var allUsers = await repository.GetAllAsync(
                new IsActiveSpec<User>(),
                new QueryProjection<User, object>(x => new { NewName = x.Name, Active = x.IsActive }));

            Assert.NotEmpty(allUsers);
            Assert.Equal(count, allUsers.Count);
            Assert.All(allUsers, _ =>
            {
                Assert.True((bool)_?.GetType().GetProperty("Active")?.GetValue(_, null));
            });
        }

        [Fact]
        public async Task Can_Get_Users_As_UserProjection()
        {
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);
            var count = userContext.Users.Count();

            var allUsers = await repository.GetAllAsync(
                new QueryProjection<User, UserProjection>(x => new UserProjection 
                { 
                    NewName = x.Name, 
                    Active = x.IsActive 
                }));

            Assert.NotEmpty(allUsers);
            Assert.Equal(count, allUsers.Count);
            Assert.All(allUsers, _ =>
            {
                Assert.False(string.IsNullOrWhiteSpace(_.NewName));
            });
        }
        #endregion

        #region Create Tests
        [Fact]
        public async Task Can_Create_New_User()
        {
            var user = new User { Name = nameof(Can_Create_New_User) };
            var userContext = _dbFixture.GetDbContext<UserContext>();
            var repository = new Repository<UserContext>(uCXTLogger.Object, userContext);

            await repository.AddAsync(user);
            await userContext.SaveChangesAsync();

            using (var context = _dbFixture.GetDbContext<UserContext>())
            {
                Assert.NotEqual(default, user.Id);

                var dbuser = await context.Users.FindAsync(user.Id);
                Assert.NotNull(dbuser);

            }
        }
        #endregion

        #region Delete Tests
        [Fact]
        public async Task Can_Delete_Users()
        {
            var user1 = new User { Name = $"{nameof(Can_Delete_Users)}1" };
            var user2 = new User { Name = $"{nameof(Can_Delete_Users)}2" };

            var userContext = _dbFixture.GetDbContext<UserContext>();
            await userContext.AddRangeAsync(user1, user2);
            await userContext.SaveChangesAsync();

            using (var context = _dbFixture.GetDbContext<UserContext>())
            {
                var repository = new Repository<UserContext>(uCXTLogger.Object, context);
                repository.Remove(user1, user2);
                await context.SaveChangesAsync();
            }

            using (var context = _dbFixture.GetDbContext<UserContext>())
            {
                var dbuser1 = await context.Users.FindAsync(user1.Id);
                var dbuser2 = await context.Users.FindAsync(user2.Id);

                Assert.Null(dbuser1);
                Assert.Null(dbuser2);
            }
        }
        #endregion

        #region Update Tests
        [Fact]
        public async Task Can_Update_Users()
        {
            var user1 = new User { Name = $"{nameof(Can_Update_Users)}1" };
            var user2 = new User { Name = $"{nameof(Can_Update_Users)}2" };

            var userContext = _dbFixture.GetDbContext<UserContext>();
            await userContext.AddRangeAsync(user1, user2);
            await userContext.SaveChangesAsync();

            using (var context = _dbFixture.GetDbContext<UserContext>())
            {
                var dbUser1 = await context.Users.FindAsync(user1.Id);
                var dbUser2 = await context.Users.FindAsync(user2.Id);

                Assert.False(dbUser1.IsActive);
                Assert.False(dbUser1.IsActive);
            }

            using (var context = _dbFixture.GetDbContext<UserContext>())
            {
                var repository = new Repository<UserContext>(uCXTLogger.Object, context);
                user1.SetIsActive(true);
                user2.SetIsActive(true);

                repository.Update(user1, user2);
                await context.SaveChangesAsync();
            }

            using (var context = _dbFixture.GetDbContext<UserContext>())
            {
                var dbUser1 = await context.Users.FindAsync(user1.Id);
                var dbUser2 = await context.Users.FindAsync(user2.Id);

                Assert.True(dbUser1.IsActive);
                Assert.True(dbUser2.IsActive);
            }

        }
        #endregion

        #region Other Tests
        [Fact]
        public async Task Can_Order_Tickets_Ascending()
        {
            using var orderContext = _dbFixture.GetDbContext<OrderContext>();
            var repository = new Repository<OrderContext>(oCXTLogger.Object, orderContext);

            var tickets = await repository.GetAllAsync(OrderBySpec<Ticket>.Ascending(x => x.TicketNumber));

            Assert.NotEmpty(tickets);
            Assert.Equal(Ticket.LOWEST_TICKET.TicketNumber, tickets.First().TicketNumber);
            Assert.Equal(Ticket.HIGH_TICKET.TicketNumber, tickets.Last().TicketNumber);
        }

        [Fact]
        public async Task Can_Order_Tickets_Descending()
        {
            using var orderContext = _dbFixture.GetDbContext<OrderContext>();
            var repository = new Repository<OrderContext>(oCXTLogger.Object, orderContext);

            var tickets = await repository.GetAllAsync(OrderBySpec<Ticket>.Descending(x => x.TicketNumber));

            Assert.NotEmpty(tickets);
            Assert.Equal(Ticket.LOWEST_TICKET.TicketNumber, tickets.Last().TicketNumber);
            Assert.Equal(Ticket.HIGH_TICKET.TicketNumber, tickets.First().TicketNumber);
        }

        [Fact]
        public async Task Can_Paginate_Results()
        {
            using (var context = _dbFixture.GetDbContext<OrderContext>())
            {
                var MAX_RETURN = 2;
                var repository = new Repository<OrderContext>(oCXTLogger.Object, context);
                var tickets = await repository.GetAllAsync(new PaginationSpec<Ticket>(1, MAX_RETURN));
                Assert.NotEmpty(tickets);
                Assert.Equal(MAX_RETURN, tickets.Count);
            }

            using (var context = _dbFixture.GetDbContext<OrderContext>())
            {
                var MAX_RETURN = 3;
                var repository = new Repository<OrderContext>(oCXTLogger.Object, context);
                var tickets = await repository.GetAllAsync(new PaginationSpec<Ticket>(1, MAX_RETURN, new IsActiveSpec<Ticket>()));
                Assert.NotEmpty(tickets);
                Assert.Equal(MAX_RETURN - 1, tickets.Count);
            }

            using (var context = _dbFixture.GetDbContext<OrderContext>())
            {
                var MAX_RETURN = 3;
                var repository = new Repository<OrderContext>(oCXTLogger.Object, context);
                var tickets = await repository.GetAllAsync(new PaginationSpec<Ticket>(1, MAX_RETURN, x => x.IsActive == false));
                Assert.NotEmpty(tickets);
                Assert.Equal(MAX_RETURN - 2, tickets.Count);
            }

            // tickets should be ordered by ascending and the first page should return the lowest ticket
            using (var context = _dbFixture.GetDbContext<OrderContext>())
            {
                var MAX_RETURN = 1;
                var repository = new Repository<OrderContext>(oCXTLogger.Object, context);
                var tickets = await repository.GetAllAsync(new PaginationSpec<Ticket>(1, MAX_RETURN, OrderBySpec<Ticket>.Ascending(x => x.TicketNumber)));
                Assert.NotEmpty(tickets);
                Assert.Equal(MAX_RETURN, tickets.Count);
                Assert.Equal(Ticket.LOWEST_TICKET.TicketNumber, tickets.First().TicketNumber);
            }

            // tickets should be ordered by ascending and the second page should return the lowest ticket
            using (var context = _dbFixture.GetDbContext<OrderContext>())
            {
                var MAX_RETURN = 1;
                var repository = new Repository<OrderContext>(oCXTLogger.Object, context);
                var tickets = await repository.GetAllAsync(new PaginationSpec<Ticket>(2, MAX_RETURN, OrderBySpec<Ticket>.Ascending(x => x.TicketNumber)));
                Assert.NotEmpty(tickets);
                Assert.Equal(MAX_RETURN, tickets.Count);
                Assert.Equal(Ticket.MIDDLE_TICKET.TicketNumber, tickets.First().TicketNumber);
            }

            // tickets should be ordered by ascending and the 3rd page should return the lowest ticket
            using (var context = _dbFixture.GetDbContext<OrderContext>())
            {
                var MAX_RETURN = 1;
                var repository = new Repository<OrderContext>(oCXTLogger.Object, context);
                var tickets = await repository.GetAllAsync(new PaginationSpec<Ticket>(3, MAX_RETURN, OrderBySpec<Ticket>.Ascending(x => x.TicketNumber)));
                Assert.NotEmpty(tickets);
                Assert.Equal(MAX_RETURN, tickets.Count);
                Assert.Equal(Ticket.HIGH_TICKET.TicketNumber, tickets.First().TicketNumber);
            }

            // tickets should return 0 as this page is outside the number of tickets created
            using (var context = _dbFixture.GetDbContext<OrderContext>())
            {
                var MAX_RETURN = 1;
                var repository = new Repository<OrderContext>(oCXTLogger.Object, context);
                var tickets = await repository.GetAllAsync(new PaginationSpec<Ticket>(99, MAX_RETURN, OrderBySpec<Ticket>.Ascending(x => x.TicketNumber)));
                Assert.Empty(tickets);
            }
        }
        #endregion
    }
}
