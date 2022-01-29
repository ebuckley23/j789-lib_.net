using J789.Library.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace J789.Library.Core.Models
{
    public class PagedResult<TData> : IPagedResult<TData>
    {
        /// <summary>
        /// Boolean indicates that there is another page of results
        /// </summary>
        public bool HasNextPage { get; protected set; }

        /// <summary>
        /// Boolean indicates that there is a previous page of results
        /// </summary>
        public bool HasPreviousPage { get; protected set; }

        /// <summary>
        /// Current page number of the requested data
        /// </summary>
        public int PageNumber { get; protected set; }

        /// <summary>
        /// Total number of pages for all the results
        /// </summary>
        public int TotalPages { get; protected set; }
        /// <summary>
        /// Collection of <typeparamref name="TData"/>
        /// </summary>
        public IReadOnlyCollection<TData> Items { get; protected set; }

        public PagedResult(IEnumerable<TData> items, int totalPages, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(totalPages / (double)pageSize);
            HasPreviousPage = PageNumber > 1;
            HasNextPage = PageNumber < TotalPages;
            Items = items.ToList();
            TotalPages = totalPages;
        }
    }
}
