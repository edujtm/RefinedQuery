using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RefinedQuery.Tests.Helpers
{
    public static class DbContextExtensions
    {   
        public static void DetachEntities(this DbContext context)
        {
            var changedEntries = context.ChangeTracker.Entries()
                .Where(e => 
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted ||
                    e.State == EntityState.Unchanged
                ).ToList();

            foreach (var entry in changedEntries)
                entry.State = EntityState.Detached;
        }
    }
}
