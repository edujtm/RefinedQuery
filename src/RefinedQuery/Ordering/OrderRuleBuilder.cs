using System;
using System.Linq.Expressions;

namespace RefinedQuery.Ordering
{
    public class OrderRuleBuilder<T>
    {
        private readonly string _fieldName;
        private readonly AbstractOrderFilter<T> _orderFilter;
        
        public OrderRuleBuilder(
            AbstractOrderFilter<T> orderFilter,
            string fieldName
        )
        {
            _fieldName = fieldName;
            _orderFilter = orderFilter;
        }
        
        public void OrderBy(Expression<Func<T, object>> fieldGetter) 
        {
            _orderFilter.OrderRules.Add(new OrderRule<T>(_fieldName, fieldGetter));
        }
    }
}