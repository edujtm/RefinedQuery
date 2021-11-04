using System;
using System.Linq.Expressions;

namespace RefinedQuery.Query
{
    public interface IQueryPredicateBuilder<T, TProperty, Q, QProperty>
    {
        void ThatMatches(Expression<Func<TProperty, QProperty, bool>> predicate);
    }
}
