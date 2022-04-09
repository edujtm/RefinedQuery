using System.Linq;

namespace RefinedQuery.Searching
{
    public interface ISearchFilter<T>
    {
        IQueryable<T> ApplySearch(IQueryable<T> values, string searchTerm);
    }
}