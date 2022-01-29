using System.Collections.Generic;

namespace J789.Library.Core.Abstraction
{
    public interface IPagedResult<TData>
    {
        /// <summary>
        /// Boolean indicates that there is another page of results
        /// </summary>
        bool HasNextPage { get; }
        /// <summary>
        /// Boolean indicates that there is a previous page of results
        /// </summary>
        bool HasPreviousPage { get; }
        /// <summary>
        /// Current page number of the requested data
        /// </summary>
        int PageNumber { get; }
        /// <summary>
        /// Total number of pages for all the results
        /// </summary>
        int TotalPages { get; }
        /// <summary>
        /// Collection of <typeparamref name="TData"/>
        /// </summary>
        IReadOnlyCollection<TData> Items { get; }
    }
}
