using System.Collections.Generic;

namespace J789.Library.Core.Abstraction
{
    public interface ICursorPagedResult<TData>
    {
        /// <summary>
        /// This is the previous cursor in relation to the current cursor
        /// </summary>
        long PreviousCursor { get; }
        /// <summary>
        /// This is the next cursor in relation to the current cursor
        /// </summary>
        long NextCursor { get; }
        /// <summary>
        /// Current cursor
        /// </summary>
        long Cursor { get; }
        /// <summary>
        /// Collection of <typeparamref name="TData"/>
        /// </summary>
        IReadOnlyCollection<TData> Items { get; }
    }
}
