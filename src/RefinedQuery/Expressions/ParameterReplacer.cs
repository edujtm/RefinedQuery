using System.Linq.Expressions;

namespace RefinedQuery.Expressions
{

    /// <summary>
    ///   Helps replacing parameters of functions with complete expressions.
    ///  Anywhere where the parameter <paramref name="From"/> appears in the body of the function is
    ///  replaced by the expression given by <paramref name="To"/>.
    /// </summary>
    class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression From;
        private readonly Expression To;

        public ParameterReplacer(ParameterExpression from, Expression to)
        {
            From = from;
            To = to;
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == From ? To : base.VisitParameter(node);
        }
    }
}