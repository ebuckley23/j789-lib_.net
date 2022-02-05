using J789.Library.Data.Abstraction.Cache;
using System;
using System.Diagnostics.CodeAnalysis;

namespace J789.Library.Data.Cache
{
    [ExcludeFromCodeCoverage]
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
