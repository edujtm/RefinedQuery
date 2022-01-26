using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using RefinedQuery.Linq;

namespace RefinedQuery.Ordering
{
    public abstract class AbstractOrderFilter<T>
    {
        public List<OrderRule<T>> OrderRules = new List<OrderRule<T>>();
        private IOrderFieldHandler orderHandler = new DefaultOrderFieldHandler();
        
        public IQueryable<T> ApplyFilter(IQueryable<T> values, IEnumerable<string> orderFields)
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
    
    public interface IOrderFieldHandler
    {
        (string Field, OrderDef Order) GetOrderFor(string queryField);
    }
    
    public class DefaultOrderFieldHandler : IOrderFieldHandler
    {
        public (string Field, OrderDef Order) GetOrderFor(string queryField)
        {
            var fields = queryField.Split(':');
            if (fields.Length == 0 || fields.Length != 2)
            {
                throw new InvalidQueryFieldException($"Cannot parse order field {queryField}.");
            }
            
            var field = fields[0];
            var orderField = fields[1];
            var order = GetOrder(orderField);
       
            return (field, order);
        }
        
        private OrderDef GetOrder(string order)
        {
            switch (order)
            {
                case "asc": return OrderDef.ASCENDING;
                case "desc": return OrderDef.DESCENDING;
                default: throw new InvalidQueryFieldException("Order is invalid");
            }
            
        }
    }
     
    public interface IOrderRuleBuilder<T>
    {
        void OrderBy<TProperty>(Expression<Func<T, TProperty>> fieldGetter);
    }
    
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
    
    public class OrderRule<T>
    {
        public string FieldName { get; }
        public Expression<Func<T, object>> FieldGetter { get; }
        
        public OrderRule(
            string fieldName,
            Expression<Func<T, object>> fieldGetter
        )
        {
            FieldName = fieldName;
            FieldGetter = fieldGetter;
        }
    }
    public class InvalidQueryFieldException : Exception
    {
        public InvalidQueryFieldException() 
            : base() {}

        public InvalidQueryFieldException(string message)
            : base(message) {}

        public InvalidQueryFieldException(string message, Exception innerException)
            : base(message, innerException) {}
    }
    
    public enum OrderDef
    {
        ASCENDING,
        DESCENDING
    }
}