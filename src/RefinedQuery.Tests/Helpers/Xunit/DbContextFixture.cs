using System;
using Microsoft.EntityFrameworkCore;

namespace RefinedQuery.Tests.Helpers.Xunit
{
    public class DbContextFixture : IDisposable
    {
        public string ConnectionString { get; } = "Server=localhost,8000;Database=TestDB;User=sa;Password=strongTest(!)Password;TrustServerCertificate=True";

        public TestDbContext Context { get; }
        public DbContextFixture()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            Context = new TestDbContext(options);
            Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
        }
    }
}