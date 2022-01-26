using System;
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
    
    public interface PaginationHandler<T, Q>
    {
        IQueryable<T> ApplyPagination(IQueryable<T> data, Q query);
    }
    
    public class OffsetAndLengthPaginationHandler<T, Q> : PaginationHandler<T, Q>
    {
        private readonly Func<Q, int> _getOffset;
        private readonly Func<Q, int> _getLength;
        
        public OffsetAndLengthPaginationHandler(
            Func<Q, int> offsetGetter,
            Func<Q, int> lengthGetter
        )
        {
            _getOffset = offsetGetter;
            _getLength = lengthGetter;
        }
        
        public IQueryable<T> ApplyPagination(IQueryable<T> data, Q query)
        {
            var offset = _getOffset(query);
            var length = _getLength(query);
            return data.Skip(offset).Take(length);
        }
    }
    
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
    
    public class PagingOptions<T, Q>
    {
        public OffsetAndLengthPaginationHandler<T, Q> OffsetAndLength(
            Func<Q, int> offset,
            Func<Q, int> length
        )
        {
            return new OffsetAndLengthPaginationHandler<T, Q>(offset, length);
        }
        
        public PageAndSizePaginationHandler<T, Q> PageAndSize(
            Func<Q, int> page,
            Func<Q, int> size
        )
        {
            return new PageAndSizePaginationHandler<T, Q>(page, size);
        }
    }
}
