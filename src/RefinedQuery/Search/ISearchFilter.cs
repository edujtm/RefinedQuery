using System;
using System.Linq.Expressions;

namespace RefinedQuery.Search
{
    public interface ISearchFilter<T> {
        Expression<Func<T, bool>> GetSearchFilter(string searchTerm);
    }
}
