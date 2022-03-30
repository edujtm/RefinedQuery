using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using RefinedQuery.Linq;
using RefinedQuery.Exceptions;

namespace RefinedQuery.Ordering
{
    public abstract class AbstractOrderFilter<T> : IOrderFilter<T>
    {
        public List<OrderRule<T>> OrderRules = new List<OrderRule<T>>();
        private IOrderFieldHandler orderHandler = new DefaultOrderFieldHandler();
        
        public IQueryable<T> ApplyOrder(IQueryable<T> values, IEnumerable<string> orderFields)
        {
            if (orderFields == null) 
            {
                throw new ArgumentException("orderFields cannot be null.");
            }
            
            if (!OrderRules.Any() || !orderFields.Any())
            {
                return values;
            }
            
            var resultQueryable = values;
            foreach (var orderField in orderFields)
            {
                var orderDef = orderHandler.GetOrderFor(orderField);
                var rule = OrderRules.FirstOrDefault(orule => orule.FieldName == orderDef.Field);
                if (rule == null)
                {
                    throw new InvalidQueryFieldException($"Ordering rule not found for field {orderField}");
                }
                
                if (orderDef.Order == OrderDef.ASCENDING)
                {
                    resultQueryable = resultQueryable.AppendOrderBy(rule.FieldGetter);
                }
                else
                {
                    resultQueryable = resultQueryable.AppendOrderByDescending(rule.FieldGetter);
                }
            }
            
            return resultQueryable;
        }
        
        public OrderRuleBuilder<T> OnField(string fieldName)
        {
            return new OrderRuleBuilder<T>(this, fieldName);
        }
    }
}