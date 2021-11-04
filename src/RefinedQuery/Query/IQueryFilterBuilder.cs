using System;
using System.Linq.Expressions;

namespace RefinedQuery.Query
{
    public interface IQueryFilterBuilder<T, Q, QProperty>
    {
        IQueryFilterBuilder<T, Q, QProperty> InvalidWhen(Expression<Func<QProperty, bool>> checkInvalid);
        IQueryFilterBuilder<T, Q, QProperty> IgnoreWhen(Expression<Func<QProperty, bool>> shouldIgnore);
        IQueryPredicateBuilder<T, TProperty, Q, QProperty> SearchFor<TProperty>(Expression<Func<T, TProperty>> propPredicate);
    }
}
