using System;
using System.Linq.Expressions;

using RefinedQuery.Expressions;

namespace RefinedQuery.Search
{
    public class SearchRule<T> : ISearchFilter<T> 
    {
        private readonly Expression<Func<T, string, bool>> _filter;

        public SearchRule(Expression<Func<T, string, bool>> filter)
        {
            _filter = filter;
        }

        public Expression<Func<T, bool>> GetSearchFilter(string searchTerm)
        {
            return _filter.Curry(searchTerm);
        }
    }
}