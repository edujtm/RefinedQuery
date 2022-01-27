using System;
using System.Linq.Expressions;

namespace RefinedQuery.Search
{
    public interface ISearchRule<T> {
        Expression<Func<T, bool>> GetSearchFilter(string searchTerm);
    }
}
