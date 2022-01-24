using System;

namespace J789.Library.Data.Abstraction.Cache
{
    public interface ICacheResult<TResult>
    {
        TResult Result { get; }
        string CacheKey { get; }
        TimeSpan TimeTaken { get; }
        bool FoundInCache { get; }
        string RawJSON { get; }
        void SetResult(TResult result);
        void SetTimeTaken(TimeSpan timeTaken);
        void SetFoundInCache(bool found);
        void SetRawJSON(string json);
    }
}
