using System;
using System.Linq;
using System.Linq.Expressions;

namespace RefinedQuery.Expressions
{
    /// <summary>
    /// Checks if an OrderBy/ThenBy method has been called in an IQueryable.
    /// <br/>
    /// <br/>
    /// The finder only analyses the IQueryable expression and does not check for
    /// the IOrderedQueryable interface. This is important because an EF Core 
    /// DbSet implements IOrderedQueryable even when the expression itself might
    /// not be ordered.
    /// </summary>
    internal class OrderingMethodFinder : ExpressionVisitor
    {
        bool _orderingMethodFound = false;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var name = node.Method.Name;

            if (node.Method.DeclaringType == typeof(Queryable) && (
                name.StartsWith("OrderBy", StringComparison.Ordinal) ||
                name.StartsWith("ThenBy", StringComparison.Ordinal)))
            {
                _orderingMethodFound = true;
            }

            return base.VisitMethodCall(node);
        }

        public static bool IsOrdered(Expression expression)
        {
            var visitor = new OrderingMethodFinder();
            visitor.Visit(expression);
            return visitor._orderingMethodFound;
        }
    }
}