using J789.Library.Data.Abstraction.Cache;
using System;
using System.Diagnostics.CodeAnalysis;

namespace J789.Library.Data.Cache
{
    [ExcludeFromCodeCoverage]
    public class CacheResult<TResult> : ICacheResult<TResult>
    {
        public CacheResult(string cacheKey, TimeSpan timeTaken, bool foundInCache)
            : this(cacheKey)
        {
            TimeTaken = timeTaken;
            FoundInCache = foundInCache;
        }

        public CacheResult(string cacheKey)
        {
            CacheKey = cacheKey;
        }

        public TResult Result { get; private set; }
        public string CacheKey { get; }
        public TimeSpan TimeTaken { get; private set; }
        public bool FoundInCache { get; private set; }
        public string RawJSON { get; private set; }
        public void SetResult(TResult result) => Result = result;
        public void SetTimeTaken(TimeSpan timeTaken) => TimeTaken = timeTaken;
        public void SetFoundInCache(bool found) => FoundInCache = found;
        public void SetRawJSON(string json) => RawJSON = json;
    }
}
