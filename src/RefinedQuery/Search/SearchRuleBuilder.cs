using System;
using System.Linq.Expressions;

using RefinedQuery.Expressions;

namespace RefinedQuery.Search
{
    public class SearchRuleBuilder<T, TProperty> : ISearchRuleBuilder<T, TProperty>
    {
        private readonly Expression<Func<T, TProperty>> _propSelector;
        private readonly AbstractSearchFilter<T> _searchFilter;

        public SearchRuleBuilder(Expression<Func<T, TProperty>> propSelector, AbstractSearchFilter<T> filter)
        {
            _propSelector = propSelector;
            _searchFilter = filter;
        }

        public void ThatMatches(Expression<Func<TProperty, string, bool>> matcher)
        {

            Expression<Func<TProperty, bool>> nullSafeGuard = (value) => value != null;
            var nonNullProp = _propSelector.Compose(nullSafeGuard);
            var completeMatcher = _propSelector.Compose(matcher);
            var safeMatcher = nonNullProp.AndAlso(completeMatcher);

            _searchFilter.SearchRules.Add(new SearchRule<T>(safeMatcher));
        }
    }
}
