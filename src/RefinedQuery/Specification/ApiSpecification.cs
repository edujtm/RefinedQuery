using System;
using System.Linq;
using System.Collections.Generic;

using RefinedQuery.Ordering;
using RefinedQuery.Pagination;
using RefinedQuery.Query;
using RefinedQuery.Search;

namespace RefinedQuery.Specification
{
    public abstract class ApiSpecification<T, Q>
    {

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

        public IQueryable<T> ApplySpecification(IQueryable<T> values, Q query)
        {
            var queryable = values;
            if (_queryFilter != null)
            {
                queryable = _queryFilter.ApplyFilter(queryable, query);
            }

            if (_searchFilter != null)
            {
                var searchTerm = _getSearchTerm(query);
                queryable = _searchFilter.ApplyFilter(queryable, searchTerm);
            }

            if (_orderFilter != null)
            {
                var orderFields = _getOrderFields(query);
                queryable = _orderFilter.ApplyFilter(queryable, orderFields);
            }

            if (_pageFilter != null)
            {
                queryable = _pageFilter.ApplyPagination(queryable, query);
            }

            return queryable;
        }
    }
}