using System;
using Microsoft.EntityFrameworkCore;

namespace RefinedQuery.Tests.Helpers.Xunit
{
    public class DbContextFixture : IDisposable
    {
        public string ConnectionString { get; } = "Data Source=localhost;Initial Catalog=TestDB;Integrated Security=True;TrustServerCertificate=True";

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