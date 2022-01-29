using J789.Library.Core.Models;
using System.Collections.Generic;
using Xunit;

namespace J789.Library.Core.UnitTests
{
    public class Paging_Tests
    {
        [Fact]
        public void PagedResult_Is_Accurate()
        {
            var items = new List<object>();
            items.Add(new object());
            items.Add(new object());
            items.Add(new object());
            items.Add(new object());

            var page = new PagedResult<object>(items, 10, 1, items.Count);

            Assert.Equal(10, page.TotalPages);
            Assert.True(page.HasNextPage);
            Assert.False(page.HasPreviousPage);
        }

        [Fact]
        public void CursorPagedResult_Is_Accurate()
        {
            var items = new List<object>();
            items.Add(new object());
            items.Add(new object());
            items.Add(new object());
            items.Add(new object());

            var page = new CursorPagedResult<object>(items, 3);
            Assert.Equal(2, page.PreviousCursor);
            Assert.Equal(4, page.NextCursor);
            Assert.Equal(3, page.Cursor);
        }
    }
}
