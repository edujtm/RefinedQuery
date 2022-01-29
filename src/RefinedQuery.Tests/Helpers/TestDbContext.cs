
using Microsoft.EntityFrameworkCore;

using RefinedQuery.Tests.Helpers.Models;

namespace RefinedQuery.Tests.Helpers
{
    public class TestDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Person>(e => {
                e.HasKey(p => p.Id);
            });
        }
    }
}