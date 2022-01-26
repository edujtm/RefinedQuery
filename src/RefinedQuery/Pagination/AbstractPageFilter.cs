using System.Linq;

namespace RefinedQuery.Pagination
{
    public abstract class AbstractPageFilter<T, Q>
    {
        private PaginationHandler<T, Q> _handler;

        // Bit of a hack to make the interface nicer
        public PagingOptions<T, Q> PagingBy { get; } = new PagingOptions<T, Q>();
        
        public IQueryable<T> ApplyPagination(IQueryable<T> values, Q query)
        {
            if (_handler == null)
            {
                return values;
            }
            
            return _handler.ApplyPagination(values, query);
        }
        
        public void WithPagination(PaginationHandler<T, Q> paginationHandler)
        {
            _handler = paginationHandler;
        }
    }
}
