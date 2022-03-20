using J789.Library.Data.Abstraction;
using J789.Library.Data.Abstraction.Cache;
using J789.Library.Data.Cache;
using JsonNet.ContractResolvers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace J789.Library.Data.Caching
{
    public class DistributedCacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<DistributedCacheRepository> _logger;
        public DistributedCacheRepository(ILogger<DistributedCacheRepository> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }
        public async Task<ICacheResult<TItem>> GetAsync<TItem>(string key, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var cacheResult = new CacheResult<TItem>(key);

            var timer = new Stopwatch();
            timer.Start();
            var val = await _cache.GetStringAsync(key);
            timer.Stop();
            cacheResult.SetTimeTaken(timer.Elapsed);

            if (string.IsNullOrEmpty(val))
            {
                _logger.LogDebug("Item not found in cache.");
                return cacheResult;
            }

            cacheResult.SetRawJSON(val);
            var result = JsonConvert.DeserializeObject<TItem>(cacheResult.RawJSON, jsonSerializerSettings ?? UseDefaultJSONSerializerSettings);
            cacheResult.SetResult(result);
            cacheResult.SetFoundInCache(true);
            return cacheResult;
        }

        public async Task<ICacheResult<TItem>> GetAsync<TItem>(string key, Func<ICacheOptions, TItem> set, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var cacheResult = await GetAsync<TItem>(key, jsonSerializerSettings);
            if (cacheResult.FoundInCache) return cacheResult;

            var options = new CacheOptions();
            var ret = set(options);
            // this is being serialized twice. TODO: Fix
            var val = JsonConvert.SerializeObject(ret);
            await SetAsync(key, ret, options);
            cacheResult.SetResult(ret);
            cacheResult.SetRawJSON(val);
            return cacheResult;
        }

        public Task<ICacheResult<TItem>> GetAsync<TItem>(string key, Func<TItem> set, JsonSerializerSettings jsonSerializerSettings = null)
            => GetAsync(key, o => set(), jsonSerializerSettings);

        public async Task SetAsync<TItem>(string key, TItem item, ICacheOptions cacheOptions = null)
        {
            if (item == null) return;

            var val = JsonConvert.SerializeObject(item);

            await _cache.SetStringAsync(key, val, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = cacheOptions?.Expiration,
                AbsoluteExpirationRelativeToNow = cacheOptions?.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = cacheOptions?.SlidingExpiration
            });
        }

        private JsonSerializerSettings UseDefaultJSONSerializerSettings => new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.Default,
            ContractResolver = new PrivateSetterAndCtorContractResolver()
        };
    }
}
