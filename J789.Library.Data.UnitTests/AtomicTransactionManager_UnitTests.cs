using J789.Library.Data.EFCore;
using J789.Library.Data.UnitTests.Fakes;
using J789.Library.Data.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace J789.Library.Data.UnitTests
{
    public class AtomicTransactionManager_UnitTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _dbFixture;
        public AtomicTransactionManager_UnitTests(DatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
        }

        [Fact]
        public async Task Can_Perform_Successful_Atomic_DB_Operation()
        {
            var user1 = new Entities.User { Name = "Emmanuel K. Buckley" };
            var order1 = new Entities.Order { Name = "My Order" };

            using (var userContext = _dbFixture.GetDbContext<UserContext>())
            using (var orderContext = _dbFixture.GetDbContext<OrderContext>())
            {
                await EFTransactionManager.New(userContext).ExecuteAsync(async (transaction) =>
                {
                    await orderContext.ShareTransactionAsync(transaction);
                    userContext.Users.Add(user1);
                    orderContext.Orders.Add(order1);
                    await orderContext.SaveChangesAsync();
                });
            }

            using (var udb = _dbFixture.GetDbContext<UserContext>())
            using (var odb = _dbFixture.GetDbContext<OrderContext>())
            {
                var dbUser1 = udb.Users.Find(user1.Id);
                var dbOrder = odb.Orders.Find(order1.Id);
                Assert.NotNull(dbUser1);
                Assert.NotNull(dbOrder);
            }
        }

        [Fact]
        public async Task Can_Perform_Successful_Rollback_If_Save_Fails()
        {
            var user1 = new Entities.User { Name = "Emmanuel K. Buckley" };
            var order1 = new Entities.Order { }; // <-- entity should cause failure due to no name specified

            using (var userContext = _dbFixture.GetDbContext<UserContext>())
            using (var orderContext = _dbFixture.GetDbContext<OrderContext>())
            {
                await Assert.ThrowsAsync<DbUpdateException>(
                    async () => await EFTransactionManager.New(userContext).ExecuteAsync(async (transaction) =>
                    {
                        await orderContext.ShareTransactionAsync(transaction);
                        userContext.Users.Add(user1);
                        orderContext.Orders.Add(order1);
                        await orderContext.SaveChangesAsync();
                    }));
            }

            using (var udb = _dbFixture.GetDbContext<UserContext>())
            using (var odb = _dbFixture.GetDbContext<OrderContext>())
            {
                var dbUser1 = udb.Users.Find(user1.Id);
                var dbOrder = odb.Orders.Find(order1.Id);
                Assert.Null(dbUser1);
                Assert.Null(dbOrder);
            }
        }
    }
}
