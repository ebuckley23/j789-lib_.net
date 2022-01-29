using J789.Library.Core.Abstraction;
using System.Collections.Generic;
using System.Linq;

namespace J789.Library.Core.Models
{
    public class CursorPagedResult<TData> : ICursorPagedResult<TData>
    {
        /// <summary>
        /// This is the previous cursor in relation to the current cursor
        /// </summary>
        public long PreviousCursor { get; protected set; }

        /// <summary>
        /// This is the next cursor in relation to the current cursor
        /// </summary>
        public long NextCursor { get; protected set; }

        /// <summary>
        /// Current cursor
        /// </summary>
        public long Cursor { get; protected set; }

        /// <summary>
        /// Collection of <typeparamref name="TData"/>
        /// </summary>
        public IReadOnlyCollection<TData> Items { get; protected set; }

        public CursorPagedResult(IEnumerable<TData> items, long cursor)
        {
            Cursor = cursor;
            NextCursor = cursor + 1;
            PreviousCursor = cursor - 1;
            Items = items.ToList();
        }
    }
}
