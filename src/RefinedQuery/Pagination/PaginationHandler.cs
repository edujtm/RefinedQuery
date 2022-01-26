using System.Linq;

namespace RefinedQuery.Pagination
{
    public interface PaginationHandler<T, Q>
    {
        IQueryable<T> ApplyPagination(IQueryable<T> data, Q query);
    }
}