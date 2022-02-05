using J789.Library.Data.EFCore.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace J789.Library.Data.EFCore
{
    public abstract class DbContextBase<TDbContext>
        : DbContext, IDbContext
        where TDbContext : DbContext
    {
        protected Guid InstanceId { get; }
        public IDbContextTransaction CurrentTransaction { get; private set; }
        public bool HasActiveTransaction => CurrentTransaction != null;

        public DbContextBase(DbContextOptions<TDbContext> options)
            : base(options)
        {
            InstanceId = Guid.NewGuid();
        }

        /// <summary>
        /// Get configured entity keys from entity
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private object[] GetKeys(EntityEntry entry)
        {
            return entry.Metadata.FindPrimaryKey()
                .Properties
                .Select(p => entry.Property(p.Name).CurrentValue)
                .ToArray();
        }

        /// <summary>
        /// Begins database transaction if transaction not already started
        /// </summary>
        /// <returns>IDbContextTransaction</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (HasActiveTransaction) return null;
            CurrentTransaction = await Database.BeginTransactionAsync();
            return CurrentTransaction;
        }

        /// <summary>
        /// Share a transaction of an IDbContext with an existing active transaction
        /// </summary>
        /// <param name="context"></param>
        /// <returns>IDbContextTransaction</returns>
        public async Task<IDbContextTransaction> ShareTransactionAsync(IDbContext context)
        {
            if (HasActiveTransaction || !context.HasActiveTransaction) return null;
            CurrentTransaction = context.CurrentTransaction;
            Database.AutoTransactionsEnabled = false;
            await Database.UseTransactionAsync(CurrentTransaction.GetDbTransaction());
            return CurrentTransaction;
        }
        /// <summary>
        /// Share a transaction from an existing transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>IDbContextTransaction</returns>
        public async Task<IDbContextTransaction> ShareTransactionAsync(IDbContextTransaction transaction)
        {
            if (HasActiveTransaction) return null;
            CurrentTransaction = transaction;
            Database.AutoTransactionsEnabled = false;
            await Database.UseTransactionAsync(transaction.GetDbTransaction());
            return CurrentTransaction;
        }

        /// <summary>
        /// // Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
        /// </summary>
        /// <see cref="https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency"/>
        public IExecutionStrategy GetExecutionStrategy() => Database.CreateExecutionStrategy();

        /// <summary>
        /// Commit database transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != CurrentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync(cancellationToken);
                transaction.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    var proposedValues = entry.CurrentValues;
                    var dbValues = await entry.GetDatabaseValuesAsync();

                    entry.CurrentValues.SetValues(dbValues);
                }
                RollbackTransaction();
                throw;
            }
            catch (Exception)
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }
        /// <summary>
        /// Rollback transaction
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                CurrentTransaction?.Rollback();
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }
    }
}
