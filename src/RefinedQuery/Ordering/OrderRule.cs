using System;
using System.Linq.Expressions;

namespace RefinedQuery.Ordering
{
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
}