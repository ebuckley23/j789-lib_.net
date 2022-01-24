using J789.Library.Data.Abstraction.Cache;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace J789.Library.Data.Abstraction
{
    public interface ICacheRepository
    {
        Task<ICacheResult<TItem>> GetAsync<TItem>(string key, JsonSerializerSettings jsonSerializerSettings = null);
        Task<ICacheResult<TItem>> GetAsync<TItem>(string key, Func<ICacheOptions, TItem> onNotFoundInCache, JsonSerializerSettings jsonSerializerSettings = null);
        Task<ICacheResult<TItem>> GetAsync<TItem>(string key, Func<TItem> onNotFoundInCache, JsonSerializerSettings jsonSerializerSettings = null);
        Task SetAsync<TItem>(
            string key,
            TItem item,
            ICacheOptions cacheOptions = null);
    }
}
