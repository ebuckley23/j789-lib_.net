using J789.Library.Data.Abstraction.Cache;
using System;

namespace J789.Library.Data.Cache
{
    public class CacheOptions : ICacheOptions
    {
        public CacheOptions()
        { }

        public CacheOptions(DateTimeOffset? expiration = default, TimeSpan? absoluteExpirationRelativeToNow = default, TimeSpan? slidingExpiration = default)
        {
            Expiration = expiration;
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            SlidingExpiration = slidingExpiration;
        }
        public DateTimeOffset? Expiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
    }
}
