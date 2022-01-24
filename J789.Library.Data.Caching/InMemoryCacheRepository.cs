using J789.Library.Data.Abstraction;
using J789.Library.Data.Abstraction.Cache;
using J789.Library.Data.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace J789.Library.Data.Caching
{
    public class InMemoryCacheRepository : ICacheRepository
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<InMemoryCacheRepository> _logger;

        public InMemoryCacheRepository(ILogger<InMemoryCacheRepository> logger, IMemoryCache cache)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<ICacheResult<TItem>> GetAsync<TItem>(string key, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var cacheResult = new CacheResult<TItem>(key);
            var timer = new Stopwatch();

            timer.Start();
            if(_cache.TryGetValue(cacheResult.CacheKey, out var result))
            {
                cacheResult.SetRawJSON(JsonConvert.SerializeObject(result, jsonSerializerSettings));
                cacheResult.SetResult((TItem)result);
                cacheResult.SetFoundInCache(true);
                timer.Stop();
                cacheResult.SetTimeTaken(timer.Elapsed);
            }

            return Task.FromResult(cacheResult as ICacheResult<TItem>);
        }

        public async Task<ICacheResult<TItem>> GetAsync<TItem>(string key, Func<ICacheOptions, TItem> set, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var cacheResult = await GetAsync<TItem>(key, jsonSerializerSettings);
            if (cacheResult.FoundInCache) return cacheResult;

            var options = new CacheOptions();
            var ret = set(options);
            var val = JsonConvert.SerializeObject(ret);
            await SetAsync(key, val, options);
            cacheResult.SetResult(ret);
            cacheResult.SetRawJSON(val);

            return cacheResult;
        }

        public Task<ICacheResult<TItem>> GetAsync<TItem>(string key, Func<TItem> set, JsonSerializerSettings jsonSerializerSettings = null)
            => GetAsync(key, set, jsonSerializerSettings);

        public Task SetAsync<TItem>(string key, TItem item, ICacheOptions cacheOptions = null)
        {
            if (item == null) return Task.FromResult(0);

            var val = JsonConvert.SerializeObject(item);
            var memCacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = cacheOptions?.Expiration,
                AbsoluteExpirationRelativeToNow = cacheOptions?.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = cacheOptions?.SlidingExpiration
            };
            _cache.Set(key, val, memCacheOptions);
            return Task.FromResult(1);
        }
    }
}
