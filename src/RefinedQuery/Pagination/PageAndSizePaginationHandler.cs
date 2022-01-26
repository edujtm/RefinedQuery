using System;
using System.Linq;

namespace RefinedQuery.Pagination
{
    public class PageAndSizePaginationHandler<T, Q> : PaginationHandler<T, Q>
    {
    
        private readonly Func<Q, int> _getPage;
        private readonly Func<Q, int> _getSize;
        
        public PageAndSizePaginationHandler(
            Func<Q, int> pageGetter,
            Func<Q, int> sizeGetter
        )
        {
            _getPage = pageGetter;
            _getSize = sizeGetter;
        }
        
        public IQueryable<T> ApplyPagination(IQueryable<T> data, Q query)
        {
            var page = _getPage(query);
            var size = _getSize(query);
            var offset = page * size;
            return data.Skip(offset).Take(size);
        }
    }
}