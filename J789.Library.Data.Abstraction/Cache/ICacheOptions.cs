using System;

namespace J789.Library.Data.Abstraction.Cache
{
    public interface ICacheOptions
    {
        DateTimeOffset? Expiration { get; set; }
        TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        TimeSpan? SlidingExpiration { get; set; }
    }
}
