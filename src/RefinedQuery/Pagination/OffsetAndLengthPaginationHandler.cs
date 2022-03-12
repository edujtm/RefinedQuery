using System;
using System.Linq;

namespace RefinedQuery.Pagination
{
    public class OffsetAndLengthPaginationHandler<T, Q> : PaginationHandler<T, Q>
    {
        private readonly Func<Q, int?> _getOffset;
        private readonly Func<Q, int?> _getLength;
        
        public OffsetAndLengthPaginationHandler(
            Func<Q, int?> offsetGetter,
            Func<Q, int?> lengthGetter
        )
        {
            _getOffset = offsetGetter;
            _getLength = lengthGetter;
        }
        
        public IQueryable<T> ApplyPagination(IQueryable<T> data, Q query)
        {
            // TODO: Allow defaults to be changed
            var offset = _getOffset(query) ?? 0;
            var length = _getLength(query) ?? 20;
            return data.Skip(offset).Take(length);
        }
    }
}