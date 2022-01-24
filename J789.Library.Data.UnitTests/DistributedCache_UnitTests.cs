using J789.Library.Data.Caching;
using J789.Library.Data.UnitTests.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace J789.Library.Data.UnitTests.Caching
{
    public class DistributedCache_UnitTests
    {
        private Mock<ILogger<DistributedCacheRepository>> logger = new Mock<ILogger<DistributedCacheRepository>>();

        [Fact]
        public async Task Can_Set_Item_In_Cache()
        {
            var repository = new DistributedCacheRepository(logger.Object, new Fakes.Cache());

            await repository.SetAsync(nameof(Can_Set_Item_In_Cache), new User { Name = "SET" });

            var itemInCache = await repository.GetAsync<User>(nameof(Can_Set_Item_In_Cache));
            Assert.NotNull(itemInCache.Result);
            Assert.Equal("SET", itemInCache.Result.Name);
        }

        [Fact]
        public async Task Can_Get_Item_From_Cache()
        {
            var repository = new DistributedCacheRepository(logger.Object, new Fakes.Cache());

            await repository.SetAsync(nameof(Can_Get_Item_From_Cache), new User() { Name = "GET" });

            var itemInCache = await repository.GetAsync<User>(nameof(Can_Get_Item_From_Cache));

            Assert.NotNull(itemInCache.Result);
            Assert.Equal("GET", itemInCache.Result.Name);
        }

        [Fact]
        public async Task Can_Set_Item_In_Cache_If_Not_Present()
        {
            var repository = new DistributedCacheRepository(logger.Object, new Fakes.Cache());

            var itemNotInCache = await repository.GetAsync(nameof(Can_Set_Item_In_Cache_If_Not_Present),
                () =>
                {
                    return new User { Name = "Should Be In Cache Now" };
                });

            Assert.NotNull(itemNotInCache.Result);
            Assert.Equal("Should Be In Cache Now", itemNotInCache.Result.Name);
        }

        [Fact]
        public async Task Can_Set_Item_In_Cache_If_Not_Present_With_Options()
        {
            var repository = new DistributedCacheRepository(logger.Object, new Fakes.Cache());

            var itemNotInCache = await repository.GetAsync(nameof(Can_Set_Item_In_Cache_If_Not_Present_With_Options),
                options =>
                {
                    options.Expiration = DateTime.UtcNow.AddDays(7);
                    return new User { Name = "Should Be In Cache w/ Options" };
                });

            Assert.NotNull(itemNotInCache.Result);
            Assert.Equal("Should Be In Cache w/ Options", itemNotInCache.Result.Name);
        }
    }
}
