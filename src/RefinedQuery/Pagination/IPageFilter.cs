using System.Linq;

namespace RefinedQuery.Pagination
{
    public interface IPageFilter<T, Q>
    {
        IQueryable<T> ApplyPagination(IQueryable<T> values, Q query);
    }
}