using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using RefinedQuery.Expressions;

namespace RefinedQuery
{

    // ------- Interno -------
    
    public interface ISearchFilter<T> {
        Expression<Func<T, bool>> GetSearchFilter(string searchTerm);
    }
    
    public interface ISearchRuleBuilder<T, TProperty> {
        void ThatMatches(Expression<Func<TProperty, string, bool>> matcher);
    }
    
    public class SearchRule<T> : ISearchFilter<T> 
    {
        private readonly Expression<Func<T, string, bool>> _filter;

        public SearchRule(Expression<Func<T, string, bool>> filter)
        {
            _filter = filter;
        }
        
        public Expression<Func<T, bool>> GetSearchFilter(string searchTerm)
        {
            return CurrySearchTerm(_filter, searchTerm);
        }

        private Expression<Func<T, bool>> CurrySearchTerm(Expression<Func<T, string, bool>> expr, string searchTerm)
        {
          var constantSearchTerm = Expression.Constant(searchTerm);
          var body = expr.Body.ReplaceParameter(expr.Parameters[1], constantSearchTerm);
          return Expression.Lambda<Func<T, bool>>(body, expr.Parameters[0]);
        }
    }
    
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Search<T>(this IQueryable<T> queryable, AbstractSearchFilter<T> matcher, string searchTerm)
        {
            return matcher.ApplyFilter(queryable, searchTerm);   
        }
    }

    public class SearchRuleBuilder<T, TProperty> : ISearchRuleBuilder<T, TProperty>
    {
        private readonly Expression<Func<T, TProperty>> _propSelector;
        private readonly AbstractSearchFilter<T> _searchFilter;
        
        public SearchRuleBuilder(Expression<Func<T, TProperty>> propSelector, AbstractSearchFilter<T> filter)
        {
            _propSelector = propSelector;
            _searchFilter = filter;
        }
        
        public void ThatMatches(Expression<Func<TProperty, string, bool>> matcher)
        {

            Expression<Func<TProperty, bool>> nullSafeGuard = (value) => value != null;
            var nonNullProp = _propSelector.Compose(nullSafeGuard);
            var completeMatcher = _propSelector.Compose(matcher);
            var safeMatcher = nonNullProp.AndAlso(completeMatcher);

            _searchFilter.SearchRules.Add(new SearchRule<T>(safeMatcher));
        }
    }
    
    public abstract class AbstractSearchFilter<T>
    {
        public List<ISearchFilter<T>> SearchRules = new List<ISearchFilter<T>>();
        
        public IQueryable<T> ApplyFilter(IQueryable<T> values, string searchTerm)
        {
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                return values;
            }

            if (!SearchRules.Any()) {
              return values;
            }
            
            Expression<Func<T, bool>> fullFilter = null;
            foreach (var rule in SearchRules)
            {
                var expr = rule.GetSearchFilter(searchTerm); 

                if (fullFilter == null) {
                  fullFilter = expr;
                } else {
                  fullFilter = fullFilter.OrElse(expr);
                }
            }
            
            return values.Where(fullFilter);
        }
        
        public ISearchRuleBuilder<T, TProperty> SearchFor<TProperty>(Expression<Func<T, TProperty>> propSelector)
        {
            var builder = new SearchRuleBuilder<T, TProperty>(propSelector, this);
            return builder;
        }
    }
}