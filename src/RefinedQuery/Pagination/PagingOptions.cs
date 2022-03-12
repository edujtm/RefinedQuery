using System;

namespace RefinedQuery.Pagination
{
    public class PagingOptions<T, Q>
    {
        public OffsetAndLengthPaginationHandler<T, Q> OffsetAndLength(
            Func<Q, int?> offset,
            Func<Q, int?> length
        )
        {
            return new OffsetAndLengthPaginationHandler<T, Q>(offset, length);
        }
        
        public PageAndSizePaginationHandler<T, Q> PageAndSize(
            Func<Q, int?> page,
            Func<Q, int?> size
        )
        {
            return new PageAndSizePaginationHandler<T, Q>(page, size);
        }
    }
}