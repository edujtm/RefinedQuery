using System.Linq.Expressions;


namespace RefinedQuery.Expressions
{
    /// <summary>
    /// Used to replace an expression in the expression with another expression.
    /// <summary>
    class ExpressionReplacer : ExpressionVisitor
    {
        private readonly Expression From;
        private readonly Expression To;

        public ExpressionReplacer(Expression from, Expression to)
        {
          From = from;
          To = to;
        }

        public override Expression Visit(Expression node)
        {
            return node == From ? To : base.Visit(node);
        }
    }
}