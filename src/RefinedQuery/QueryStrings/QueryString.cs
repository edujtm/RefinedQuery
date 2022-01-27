using System;
using System.Linq;
using System.Collections.Generic;

using RefinedQuery.Ordering;
using RefinedQuery.Pagination;
using RefinedQuery.Query;
using RefinedQuery.Search;

namespace RefinedQuery.QueryStrings
{
    public abstract class QueryString<T, Q> 
        : IPageFilter<T, Q>,
          IQueryFilter<T, Q>,
          IOrderFilter<T>,
          ISearchFilter<T>
    {

        // Same hack from the AbstractPageFilter but for the full query string
        public PagingOptions<T, Q> PagingBy { get; } = new PagingOptions<T, Q>();

        private AbstractQueryFilter<T, Q> _queryFilter = null;

        private Func<Q, string> _getSearchTerm = null;
        private AbstractSearchFilter<T> _searchFilter = null;

        private Func<Q, IEnumerable<string>> _getOrderFields = null;
        private AbstractOrderFilter<T> _orderFilter = null;

        private AbstractPageFilter<T, Q> _pageFilter = null; 

        public void UseQuery(Action<AbstractQueryFilter<T, Q>> queryingBuilder)
        {
            var queryFilter = new DefaultQueryFilter<T, Q>();
            queryingBuilder(queryFilter);
            _queryFilter = queryFilter;
        }

        public void UseSearch(
            Func<Q, string> searchTerm,
            Action<AbstractSearchFilter<T>> searchBuilder
        )
        {
            var searchFilter = new DefaultSearchFilter<T>();
            searchBuilder(searchFilter);

            _getSearchTerm = searchTerm;
            _searchFilter = searchFilter;
        }

        public void UseOrdering(
            Func<Q, IEnumerable<string>> orderFields,
            Action<AbstractOrderFilter<T>> orderingBuilder
        )
        {
            var orderFilter = new DefaultOrderFilter<T>();
            orderingBuilder(orderFilter);

            _getOrderFields = orderFields;
            _orderFilter = orderFilter;
        }

        public void UsePagination(Action<AbstractPageFilter<T, Q>> paginationBuilder)
        {
            var pageFilter = new DefaultPageFilter<T, Q>();
            paginationBuilder(pageFilter);
            _pageFilter = pageFilter;
        }

        public IQueryable<T> ApplyQuery(IQueryable<T> values, Q query)
            => _queryFilter != null ? _queryFilter.ApplyQuery(values, query) : values;

        public IQueryable<T> ApplySearch(IQueryable<T> values, string searchTerm)
            => _searchFilter != null ? _searchFilter.ApplySearch(values, searchTerm) : values;

        public IQueryable<T> ApplyOrder(IQueryable<T> values, IEnumerable<string> orderDefs)
            => _orderFilter != null ? _orderFilter.ApplyOrder(values, orderDefs) : values;

        public IQueryable<T> ApplyPagination(IQueryable<T> values, Q query) 
            => _pageFilter != null ? _pageFilter.ApplyPagination(values, query) : values;

        public IQueryable<T> ApplyQueryString(IQueryable<T> values, Q query)
        {
            var queryable = values;
            if (_queryFilter != null)
            {
                queryable = _queryFilter.ApplyQuery(queryable, query);
            }

            if (_searchFilter != null)
            {
                var searchTerm = _getSearchTerm(query);
                queryable = _searchFilter.ApplySearch(queryable, searchTerm);
            }

            if (_orderFilter != null)
            {
                var orderFields = _getOrderFields(query);
                queryable = _orderFilter.ApplyOrder(queryable, orderFields);
            }

            if (_pageFilter != null)
            {
                queryable = _pageFilter.ApplyPagination(queryable, query);
            }

            return queryable;
        }
    }
}