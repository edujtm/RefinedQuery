using System;
using System.Linq.Expressions;


namespace RefinedQuery.Query
{
    public interface IQueryRule<T, Q>
    {
        bool CheckInvalid(Q query);
        bool ShouldIgnore(Q query);
        Expression<Func<T, bool>> GetFilter(Q query);
    }
}
