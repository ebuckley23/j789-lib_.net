using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace J789.Library.Data.UnitTests.Fakes
{
    class Entry : ICacheEntry
    {
        public Entry(object key)
        {
            Key = key;
        }
        public Entry(object key, object value)
            : this(key)
        {
            Value = value;
        }
        public object Key { get; private set; }

        public object Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }

        public IList<IChangeToken> ExpirationTokens => throw new NotImplementedException();

        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks => throw new NotImplementedException();

        public CacheItemPriority Priority { get; set; }
        public long? Size { get; set; }

        public void Dispose()
        {
            
        }
    }
    public class Cache : IDistributedCache, IMemoryCache
    {
        private static Dictionary<object, dynamic> _cache = new Dictionary<object, dynamic>();

        public ICacheEntry CreateEntry(object key)
        {
            if (!_cache.ContainsKey(key))
            {
                var ret = new Entry(key);
                _cache.Add(ret, ret.Value);
            }

            return new Entry(null);
        }

        public void Dispose()
        {
        }

        public byte[] Get(string key)
        {
            if (_cache.TryGetValue(key, out var val))
            {
                return val;
            }
            return null;
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            return Task.FromResult(Get(key));
        }

        public void Refresh(string key)
        {
            var val = Get(key);
            Set(key, val, null);
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            var val = await GetAsync(key, token);
            await SetAsync(key, val, null, token);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Remove(key);
            return Task.FromResult(0);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var item = Get(key);
            if (item != null)
            {
                _cache[key] = value;
            }
            else
            {
                _cache.TryAdd(key, value);
            }
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            Set(key, value, options);
            return Task.FromResult(0);
        }

        public bool TryGetValue(object key, out object value)
        {
            if(_cache.TryGetValue(key, out var r))
            {
                value = r;
                return true;
            }
            value = null;
            return false;
        }
    }
}
