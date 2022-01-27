using System.Linq;

namespace RefinedQuery.Query
{
    public interface IQueryFilter<T, Q> 
    {
        IQueryable<T> ApplyQuery(IQueryable<T> values, Q query);
    }
}