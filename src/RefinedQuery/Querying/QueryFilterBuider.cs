using System;
using System.Linq.Expressions;

namespace RefinedQuery.Querying
{
    public class QueryFilterBuilder<T, Q, QProperty> : IQueryFilterBuilder<T, Q, QProperty>
    {
        private readonly Expression<Func<Q, QProperty>> _queryPropSelector;
        private readonly AbstractQueryFilter<T, Q> _queryFilter;

        private Expression<Func<QProperty, bool>> _shouldIgnore = (value) => value == null;
        private Expression<Func<QProperty, bool>> _checkInvalid = (value) => false;

        public QueryFilterBuilder(
            Expression<Func<Q, QProperty>> queryPropSelector,
            AbstractQueryFilter<T, Q> queryFilter
        )
        {
            _queryPropSelector = queryPropSelector;
            _queryFilter = queryFilter;
        }

        public IQueryFilterBuilder<T, Q, QProperty> IgnoreWhen(Expression<Func<QProperty, bool>> shouldIgnore)
        {
            _shouldIgnore = shouldIgnore;
            return this;
        }

        public IQueryFilterBuilder<T, Q, QProperty> InvalidWhen(Expression<Func<QProperty, bool>> checkInvalid)
        {
            _checkInvalid = checkInvalid;
            return this;
        }

        public IQueryPredicateBuilder<T, TProperty, Q, QProperty> SearchFor<TProperty>(Expression<Func<T, TProperty>> propSelector)
        {
            return new QueryPredicateBuilder<T, TProperty, Q, QProperty>(
                propSelector,
                _queryPropSelector,
                _shouldIgnore,
                _checkInvalid,
                _queryFilter
            );
        }
    }
}
