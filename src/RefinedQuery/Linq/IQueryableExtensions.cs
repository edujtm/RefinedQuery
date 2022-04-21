using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using RefinedQuery.Querying;
using RefinedQuery.Searching;
using RefinedQuery.Ordering;
using RefinedQuery.Pagination;
using RefinedQuery.QueryHandler;
using RefinedQuery.Expressions;

namespace RefinedQuery.Linq
{
    public static class IQueryableExtensions
    {

        /// <summary>
        /// Queries the IQueryable using the rules defined in the Query filter. <br/>
        /// 
        /// The rules are AND'ed together so that only items that match all of rules
        /// remains in the queryable. <br/>
        ///
        /// NOTE: when used with QueryHandler<> object, this method only applies the
        /// query section of the rule.
        /// </summary>
        /// <param name="queryable">The queryable that will be queried for a match</param>
        /// <param name="queryFilter">the query filter with filtering rules</param>
        /// <param name="query">object with the information that will be used to query the queryable</param>
        /// <returns>Queryable with only the object that matches all the rules in the query filter</returns>
        public static IQueryable<T> QueryBy<T, Q>(
            this IQueryable<T> queryable, 
            IQueryFilter<T, Q> queryFilter, 
            Q query
        )
        {
            return queryFilter.ApplyQuery(queryable, query);
        }

        /// <summary>
        /// Searches the IQueryable using the search rules in the search filter. <br/>
        ///
        /// The rules are OR'ed together so that items that matches any rules are kept
        /// in the resulting queryable.
        /// </summary>
        /// <param name="queryable">The queryable that will be searched for matches</param>
        /// <param name="searchFilter">The search filter with rules for searching</param>
        /// <param name="searchTerm">The term that will be used to search the queryable</param>
        /// <returns>Queryable with all the objects that match any of the rules in the search filter</returns>
        public static IQueryable<T> SearchBy<T>(
            this IQueryable<T> queryable, 
            ISearchFilter<T> searchFilter, 
            string searchTerm
        )
        {
            return searchFilter.ApplySearch(queryable, searchTerm);   
        }

        /// <summary>
        /// Applies pagination to the queryable using the rules defined in the page filter.
        /// </summary>
        /// <param name="queryable">The queryable that will be paginated</param>
        /// <param name="pageFilter">The page filter with rules for pagination</param>
        /// <param name="query">object with necessary data for the pagination</param>
        /// <returns>Queryable paged by applying the query to the rules in the page filter</returns>
        public static IQueryable<T> PaginateBy<T, Q>(
            this IQueryable<T> queryable, 
            IPageFilter<T, Q> pageFilter, 
            Q query
        )
        {
            return pageFilter.ApplyPagination(queryable, query);
        }

        /// <summary>
        /// Sorts the queryable using the rules defined in the order filter.
        /// </summary>
        /// <param name="queryable">The queryable that will be ordered</param>
        /// <param name="orderFilter">The order filter with ordering rules</param>
        /// <param name="orderFields">fields from the query string that define the ordering</param>
        /// <returns>Queryable ordered by applying the order fields to the rules in the filter</returns>
        public static IQueryable<T> OrderBy<T>(
            this IQueryable<T> queryable, 
            IOrderFilter<T> orderFilter, 
            IEnumerable<string> orderFields
        )
        {
            return orderFilter.ApplyOrder(queryable, orderFields);
        }

        /// <summary>
        /// Applies all the rules defined in the Query Handler definition
        /// to the given queryable.
        /// </summary>
        /// <param name="queryable">Queryable that will be queried using the query string</param>
        /// <param name="queryHandler">object with rules for parsing the query string</param>
        /// <param name="query">Object with query data</param>
        /// <returns>Queryable transformed by applying the query data to the querying rules</returns>
        public static IQueryable<T> Apply<T, Q>(
            this IQueryable<T> queryable,
            QueryHandler<T, Q> queryHandler,
            Q query
        )
        {
            return queryHandler.Apply(queryable, query);
        }

        internal static IOrderedQueryable<T> AppendOrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector)
            => query.Expression.Type == typeof(IOrderedQueryable<T>)
            ? ((IOrderedQueryable<T>) query).ThenBy(keySelector)
            : query.OrderBy(keySelector);

        internal static IOrderedQueryable<T> AppendOrderByDescending<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector)
            => query.Expression.Type == typeof(IOrderedQueryable<T>)
                ? ((IOrderedQueryable<T>)query).ThenByDescending(keySelector)
                : query.OrderByDescending(keySelector);

        internal static bool IsOrdered<T>(this IQueryable<T> queryable)
            => OrderingMethodFinder.IsOrdered(queryable.Expression);
    }
}
