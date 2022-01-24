using J789.Library.Data.UnitTests.Fakes;
using J789.Library.Data.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace J789.Library.Data.UnitTests
{
    public class BaseDbContext_UnitTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _dbFixture;
        public BaseDbContext_UnitTests(DatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
        }

        [Fact]
        public async Task Can_Add_Data_Via_Transaction()
        {
            var user1 = new Entities.User { Name = "Emmanuel K. Buckley" };
            var user2 = new Entities.User { Name = "John Simpson" };
            using (var db = _dbFixture.GetDbContext<UserContext>())
            {
                var strategy = db.GetExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var userContext = _dbFixture.GetDbContext<UserContext>())
                    {
                        using (var transaction = await userContext.BeginTransactionAsync())
                        {
                            userContext.Users.Add(user1);
                            userContext.Users.Add(user2);

                            await userContext.CommitTransactionAsync(transaction);
                        }
                    }
                });
            }

            using (var db = _dbFixture.GetDbContext<UserContext>())
            {
                var dbUser1 = db.Users.Find(user1.Id);
                var dbUser2 = db.Users.Find(user2.Id);
                Assert.NotNull(dbUser1);
                Assert.NotNull(dbUser2);
                Assert.Equal(user1.Name, dbUser1.Name);
                Assert.Equal(user2.Name, dbUser2.Name);
            }
        }

        [Fact]
        public async Task Can_Rollback_Transaction()
        {
            var user1 = new Entities.User { Name = "Emmanuel K. Buckley" };
            var order1 = new Entities.Order { }; // <-- entity should cause failure due to no name specified

            using (var orderContext = _dbFixture.GetDbContext<OrderContext>())
            {
                orderContext.Orders.Add(order1);
                var strategy = orderContext.GetExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await orderContext.BeginTransactionAsync())
                    {
                        using (var userContext = _dbFixture.GetDbContext<UserContext>())
                        {
                            userContext.Users.Add(user1);
                            await userContext.ShareTransactionAsync(orderContext);
                            await userContext.SaveChangesAsync();
                            try
                            {
                                await orderContext.CommitTransactionAsync(transaction);
                            }
                            catch { }
                        }
                    }
                });
            }

            using (var odb = _dbFixture.GetDbContext<OrderContext>())
            using (var udb = _dbFixture.GetDbContext<UserContext>())
            {
                var dbUser1 = udb.Users.Find(user1.Id);
                var dbOrder = odb.Orders.Find(order1.Id);
                Assert.Null(dbUser1);
                Assert.Null(dbOrder);
            }
        }
    }
}
