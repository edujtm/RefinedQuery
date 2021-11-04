using System;
using System.Linq.Expressions;

namespace RefinedQuery.Search
{
    public interface ISearchRuleBuilder<T, TProperty> {
        void ThatMatches(Expression<Func<TProperty, string, bool>> matcher);
    }
}
