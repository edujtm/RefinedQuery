using System.Linq;

namespace RefinedQuery.Querying
{
    public interface IQueryFilter<T, Q> 
    {
        IQueryable<T> ApplyQuery(IQueryable<T> values, Q query);
    }
}