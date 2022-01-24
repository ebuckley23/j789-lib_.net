using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace J789.Library.Data.EFCore.Abstraction
{
    public interface IDbContext
    {
        bool HasActiveTransaction { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
        void RollbackTransaction();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IExecutionStrategy GetExecutionStrategy();
        IDbContextTransaction CurrentTransaction { get; }
    }
}
