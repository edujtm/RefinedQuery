using System;
using System.Linq.Expressions;

using RefinedQuery.Expressions;

namespace RefinedQuery.Searching
{
    public class SearchRule<T> : ISearchRule<T> 
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
