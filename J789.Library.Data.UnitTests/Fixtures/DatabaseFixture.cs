using J789.Library.Data.EFCore.Abstraction;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace J789.Library.Data.UnitTests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        public readonly SqliteConnection _sqliteConnection;
        private readonly Dictionary<string, DbContextOptions> _configuredContexts = new Dictionary<string, DbContextOptions>();
        public DatabaseFixture()
        {
            try
            {
                _sqliteConnection = new SqliteConnection("DataSource=:memory:");
                _sqliteConnection.Open();

                var test = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(x => typeof(IDbContext).IsAssignableFrom(x) && x.IsClass)
                    .ToList();

                foreach(var contextType in test)
                {
                    var optionsBuilder = typeof(DbContextOptionsBuilder<>).MakeGenericType(contextType);
                    var options = ((DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilder))
                        .UseSqlite(
                        _sqliteConnection,
                        options =>
                        {
                            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        })
                        .EnableSensitiveDataLogging()
                        .Options;

                    var context = (DbContext)Activator.CreateInstance(contextType, options);

                    var script = context.Database.GenerateCreateScript();
                    context.Database.ExecuteSqlRaw(script);

                    _configuredContexts.Add(contextType.Name, options);

                }
            }
            catch (Exception)
            {
                _sqliteConnection.Close();
                throw;
            }
        }

        public TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext
        {
            DbContextOptions options = null;
            if (_configuredContexts.TryGetValue(typeof(TDbContext).Name, out options))
            {
                return (TDbContext)Activator.CreateInstance(typeof(TDbContext), options);
            }
            else
            {
                options = new DbContextOptionsBuilder<TDbContext>()
                    .UseSqlite(
                        _sqliteConnection,
                        options =>
                        {
                            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        })
                    .EnableSensitiveDataLogging()
                    .Options;

                _configuredContexts.Add(typeof(TDbContext).Name, options);

                var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), options);

                var script = context.Database.GenerateCreateScript();
                context.Database.ExecuteSqlRaw(script);

                return context;
            }
        }
        public void Dispose()
        {
            _sqliteConnection.Close();
        }
    }
}
