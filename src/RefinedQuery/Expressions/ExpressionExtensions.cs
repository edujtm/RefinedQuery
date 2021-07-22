using System;
using System.Linq.Expressions;

namespace RefinedQuery.Expressions
{
    public static class ExpressionExtensions
    {
        public static Expression ReplaceParameter(this Expression expression, ParameterExpression source, Expression target)
        {
            return new ParameterReplacer(source, target).Visit(expression);
        }

        public static TExpr ReplaceExpressions<TExpr>(this TExpr expression, Expression orig, Expression replacement)
            where TExpr : Expression
        {
            var replacer = new ExpressionReplacer(orig, replacement); 
            return replacer.VisitAndConvert(expression, nameof(ReplaceExpressions));
        }

        public static Expression<Func<A, C, D>> Compose<A, B, C, D>(this Expression<Func<A, B>> f, Expression<Func<B, C, D>> g)
        {
            var newExpr = g.Body.ReplaceExpressions(g.Parameters[0], f.Body);
            return Expression.Lambda<Func<A, C, D>>(newExpr, f.Parameters[0], g.Parameters[1]);
        }

        public static Expression<Func<A, C>> Compose<A, B, C>(this Expression<Func<A, B>> f, Expression<Func<B, C>> g)
        {
            var newExpr = g.Body.ReplaceExpressions(g.Parameters[0], f.Body);
            return Expression.Lambda<Func<A, C>>(newExpr, f.Parameters[0]);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> f, Expression<Func<T, bool>> g)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ExpressionReplacer(f.Parameters[0], parameter);
            var left = leftVisitor.Visit(f.Body);

            var rigthVisitor = new ExpressionReplacer(g.Parameters[0], parameter);
            var right = rigthVisitor.Visit(g.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
        }

        public static Expression<Func<T, V, bool>> AndAlso<T, V>(this Expression<Func<T, bool>> f, Expression<Func<T, V, bool>> g)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ExpressionReplacer(f.Parameters[0], parameter);
            var left = leftVisitor.Visit(f.Body);

            var rigthVisitor = new ExpressionReplacer(g.Parameters[0], parameter);
            var right = rigthVisitor.Visit(g.Body);

            return Expression.Lambda<Func<T, V, bool>>(Expression.AndAlso(left, right), parameter, g.Parameters[1]);
        }
    }
}