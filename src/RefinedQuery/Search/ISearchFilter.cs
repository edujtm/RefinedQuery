using System.Linq;

namespace RefinedQuery.Search
{
    public interface ISearchFilter<T>
    {
        IQueryable<T> ApplySearch(IQueryable<T> values, string searchTerm);
    }
}