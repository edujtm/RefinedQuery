using System;
using System.Linq;
using System.Linq.Expressions;

using RefinedQuery.Search;

namespace RefinedQuery.Linq
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Search<T>(this IQueryable<T> queryable, AbstractSearchFilter<T> matcher, string searchTerm)
        {
            return matcher.ApplySearch(queryable, searchTerm);   
        }

        internal static IOrderedQueryable<T> AppendOrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector)
            => query.Expression.Type == typeof(IOrderedQueryable<T>)
            ? ((IOrderedQueryable<T>) query).ThenBy(keySelector)
            : query.OrderBy(keySelector);

        internal static IOrderedQueryable<T> AppendOrderByDescending<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector)
            => query.Expression.Type == typeof(IOrderedQueryable<T>)
                ? ((IOrderedQueryable<T>)query).ThenByDescending(keySelector)
                : query.OrderByDescending(keySelector);
    }
}
