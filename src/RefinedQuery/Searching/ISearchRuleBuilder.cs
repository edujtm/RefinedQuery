using System;
using System.Linq.Expressions;

namespace RefinedQuery.Searching
{
    public interface ISearchRuleBuilder<T, TProperty> {
        void ThatMatches(Expression<Func<TProperty, string, bool>> matcher);
    }
}
