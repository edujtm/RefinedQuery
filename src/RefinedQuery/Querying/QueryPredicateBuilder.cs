using System;
using System.Linq.Expressions;

using RefinedQuery.Expressions;

namespace RefinedQuery.Querying
{
    /// Def pissed off some generic typing gods while making this class. 
    public class QueryPredicateBuilder<T, TProperty, Q, QProperty> : IQueryPredicateBuilder<T, TProperty, Q, QProperty>
    {
        private readonly Expression<Func<T, TProperty>> _propSelector;
        private readonly Expression<Func<Q, QProperty>> _queryPropSelector;
        private readonly Expression<Func<QProperty, bool>> _shouldIgnore;
        private readonly Expression<Func<QProperty, bool>> _checkInvalid;
        private readonly AbstractQueryFilter<T, Q> _queryFilter;

        public QueryPredicateBuilder(
           Expression<Func<T, TProperty>> propSelector,
           Expression<Func<Q, QProperty>> queryPropSelector,
           Expression<Func<QProperty, bool>> shouldIgnore,
           Expression<Func<QProperty, bool>> checkInvalid,
           AbstractQueryFilter<T, Q> queryFilter
        )
        {
            _propSelector = propSelector;
            _queryPropSelector = queryPropSelector;
            _shouldIgnore = shouldIgnore;
            _checkInvalid = checkInvalid;
            _queryFilter = queryFilter;
        }

        public void ThatMatches(Expression<Func<TProperty, QProperty, bool>> predicate)
        {
            var halfPredicate = _propSelector.Compose(predicate);
            var fullPredicate = _queryPropSelector.ComposeSecond(halfPredicate);

            var completeShouldIgnore = _queryPropSelector.Compose(_shouldIgnore);
            var completeCheckInvalid = _queryPropSelector.Compose(_checkInvalid);

            _queryFilter.QueryRules.Add(new QueryRule<T, Q>(fullPredicate, completeShouldIgnore, completeCheckInvalid));
        }
    }
}
