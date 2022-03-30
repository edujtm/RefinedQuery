
using RefinedQuery.Exceptions;

namespace RefinedQuery.Ordering
{
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
}