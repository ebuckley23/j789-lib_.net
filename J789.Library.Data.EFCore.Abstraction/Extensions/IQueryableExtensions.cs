using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace J789.Library.Data.EFCore.Abstraction.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> TagWithSource<T>(this IQueryable<T> queryable,
            string tag = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            return queryable.TagWith($"{tag}{Environment.NewLine}{memberName} - {filePath}:{lineNumber}");
        }
    }
}
