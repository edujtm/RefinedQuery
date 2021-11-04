using System.Linq;

using RefinedQuery.Search;

namespace RefinedQuery.Linq
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Search<T>(this IQueryable<T> queryable, AbstractSearchFilter<T> matcher, string searchTerm)
        {
            return matcher.ApplyFilter(queryable, searchTerm);   
        }
    }
}
