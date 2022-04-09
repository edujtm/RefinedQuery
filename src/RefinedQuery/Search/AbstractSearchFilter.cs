using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using RefinedQuery.Expressions;

namespace RefinedQuery.Searching
{

    public abstract class AbstractSearchFilter<T> : ISearchFilter<T>
    {
        public List<ISearchRule<T>> SearchRules = new List<ISearchRule<T>>();

        public IQueryable<T> ApplySearch(IQueryable<T> values, string searchTerm)
        {
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                return values;
            }

            if (!SearchRules.Any()) {
              return values;
            }

            Expression<Func<T, bool>> fullFilter = null;
            foreach (var rule in SearchRules)
            {
                var expr = rule.GetSearchFilter(searchTerm); 

                if (fullFilter == null) {
                  fullFilter = expr;
                } else {
                  fullFilter = fullFilter.OrElse(expr);
                }
            }

            return values.Where(fullFilter);
        }

        public ISearchRuleBuilder<T, TProperty> SearchFor<TProperty>(Expression<Func<T, TProperty>> propSelector)
        {
            var builder = new SearchRuleBuilder<T, TProperty>(propSelector, this);
            return builder;
        }
    }
}
