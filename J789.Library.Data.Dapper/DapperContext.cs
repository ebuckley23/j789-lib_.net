using System;
using System.Data;

namespace J789.Library.Data.Dapper
{
    public class DapperContext
    {
        protected Guid InstanceId { get; }
        public IDbTransaction CurrentTransaction { get; private set; }
        public bool HasActiveTransaction => CurrentTransaction != null;
        public IDbConnection Database { get; private set; }

        public DapperContext(DapperContextOptions options)
        {
            InstanceId = Guid.NewGuid();
        }

        /// <summary>
        /// Begins database transaction if transaction not already started
        /// </summary>
        /// <returns>IDbTransaction</returns>
        public IDbTransaction BeginTransaction()
        {
            if (HasActiveTransaction) return null;
            CurrentTransaction = Database.BeginTransaction();
            return CurrentTransaction;
        }
    }

    public class DapperContextOptions
    {

    }
}
