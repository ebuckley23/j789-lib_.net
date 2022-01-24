using J789.Library.Data.EFCore.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace J789.Library.Data.EFCore
{
    /// <summary>
    /// <see cref="https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency"/>
    /// </summary>
    public class EFTransactionManager
    {
        private IDbContext _context;
        private EFTransactionManager(IDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public static EFTransactionManager New(IDbContext context) => new EFTransactionManager(context);

        /// <summary>
        /// Contexts must share the same database connections or this will not work
        /// <see cref="https://github.com/dotnet/runtime/issues/715""/>
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(Func<IDbContextTransaction, Task> action)
        {
            // Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            // See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            var strategy = _context.GetExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.BeginTransactionAsync())
                {
                    await action(transaction);
                    await _context.CommitTransactionAsync(transaction);
                }
            });
        }
    }
}
