using System.Linq;

namespace RefinedQuery.Pagination
{
    public abstract class AbstractPageFilter<T, Q> : IPageFilter<T, Q>
    {
        private PaginationHandler<T, Q> _handler;

        // Bit of a hack to make the interface nicer
        // This should have been a static class, but having it as an instance property
        // allows the types to be inferred from the context.
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
