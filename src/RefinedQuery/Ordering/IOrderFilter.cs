using System.Collections.Generic;
using System.Linq;

namespace RefinedQuery.Ordering
{
    public interface IOrderFilter<T>
    {
        IQueryable<T> ApplyOrder(IQueryable<T> values, IEnumerable<string> orderFields);
    }
}