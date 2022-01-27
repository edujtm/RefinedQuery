using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using RefinedQuery.Expressions;

namespace RefinedQuery.Query
{

    public abstract class AbstractQueryFilter<T, Q> : IQueryFilter<T, Q>
    {
        public List<IQueryRule<T, Q>> QueryRules = new List<IQueryRule<T, Q>>();

        public IQueryable<T> ApplyQuery(IQueryable<T> values, Q query)
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
}
