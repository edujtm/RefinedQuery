using System;
using System.Linq;

namespace RefinedQuery.Pagination
{
    public class PageAndSizePaginationHandler<T, Q> : PaginationHandler<T, Q>
    {
    
        private readonly Func<Q, int?> _getPage;
        private readonly Func<Q, int?> _getSize;
        
        public PageAndSizePaginationHandler(
            Func<Q, int?> pageGetter,
            Func<Q, int?> sizeGetter
        )
        {
            _getPage = pageGetter;
            _getSize = sizeGetter;
        }
        
        public IQueryable<T> ApplyPagination(IQueryable<T> data, Q query)
        {
            // TODO: make the default values customizable
            int page = _getPage(query) ?? 0;
            int size = _getSize(query) ?? 20;
            var offset = page * size;
            return data.Skip(offset).Take(size);
        }
    }
}