using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using RefinedQuery.Expressions;

namespace RefinedQuery.QueryFilter
{

    public class Person
    {
        public string FirstName { get; set; }

        public int Age { get; set; }
    }

    public class PersonQuery
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class InvalidQueryStringException : Exception
    {
        public InvalidQueryStringException() 
            : base() {}

        public InvalidQueryStringException(string message)
            : base(message) {}

        public InvalidQueryStringException(string message, Exception innerException)
            : base(message, innerException) {}
    }

    public interface IQueryRule<T, Q>
    {
        bool CheckInvalid(Q query);
        bool ShouldIgnore(Q query);
        Expression<Func<T, bool>> GetFilter(Q query);
    }

    public interface IQueryFilterBuilder<T, Q, QProperty>
    {
        IQueryFilterBuilder<T, Q, QProperty> InvalidWhen(Expression<Func<QProperty, bool>> checkInvalid);
        IQueryFilterBuilder<T, Q, QProperty> IgnoreWhen(Expression<Func<QProperty, bool>> shouldIgnore);
        IQueryPredicateBuilder<T, TProperty, Q, QProperty> SearchFor<TProperty>(Expression<Func<T, TProperty>> propPredicate);
    }

    public interface IQueryPredicateBuilder<T, TProperty, Q, QProperty>
    {
        void ThatMatches(Expression<Func<TProperty, QProperty, bool>> predicate);
    }

    public class PersonQueryFilter : AbstractQueryFilter<Person, PersonQuery>
    {
        public PersonQueryFilter()
        {
            OnQuery((q) => q.Name)
              .IgnoreWhen((query) => string.IsNullOrWhiteSpace(query))
              .SearchFor((p) => p.FirstName)
              .ThatMatches((name, query) => name.ToLower().Contains(query.ToLower()));

            OnQuery((q) => q.Age)
              .InvalidWhen((age) => age <= 0)
              .SearchFor((p) => p.Age)
              .ThatMatches((age, query) => age == query);
        }
    }
    
    public abstract class AbstractQueryFilter<T, Q>
    {
        public List<IQueryRule<T, Q>> QueryRules = new List<IQueryRule<T, Q>>();

        public IQueryable<T> ApplyFilter(IQueryable<T> values, Q query)
        {
            if (!QueryRules.Any())
            {
                return values;
            }

            Expression<Func<T, bool>> compositeFilter = null;
            foreach (var rule in QueryRules)
            {
                if (rule.CheckInvalid(query))
                {
                    throw new InvalidQueryStringException("Query is invalid");
                }

                if (rule.ShouldIgnore(query))
                {
                    continue;
                }

                var expr = rule.GetFilter(query);
                if (compositeFilter == null)
                {
                    compositeFilter = expr;
                } else {
                    compositeFilter = compositeFilter.AndAlso(expr);
                }
            }

            return compositeFilter != null ? values.Where(compositeFilter) : values;
        }

        public IQueryFilterBuilder<T, Q, QProperty> OnQuery<QProperty>(Expression<Func<Q, QProperty>> queryPropSelector)
        {
            var builder = new QueryFilterBuilder<T, Q, QProperty>(queryPropSelector, this);
            return builder;
        }
    }
    
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

    public class QueryRule<T, Q> : IQueryRule<T, Q>
    {
        private readonly Expression<Func<T, Q, bool>> _predicate;
        private readonly Func<Q, bool> _shouldIgnore;
        private readonly Func<Q, bool> _checkInvalid;

        public QueryRule(
            Expression<Func<T, Q, bool>> predicate,
            Expression<Func<Q, bool>> shouldIgnore,
            Expression<Func<Q, bool>> checkInvalid
        )
        {
            _predicate = predicate;
            _shouldIgnore = shouldIgnore.Compile();
            _checkInvalid = checkInvalid.Compile();
        }

        public bool CheckInvalid(Q query) => _checkInvalid(query);
        public bool ShouldIgnore(Q query) => _shouldIgnore(query);

        public Expression<Func<T, bool>> GetFilter(Q query)
        {
            return _predicate.Curry(query);
        }
    }
}
