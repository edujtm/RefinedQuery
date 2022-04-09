using System;
using System.Linq.Expressions;

using RefinedQuery.Expressions;

namespace RefinedQuery.Querying
{
    public class QueryRule<T, Q> : IQueryRule<T, Q>
    {
        private readonly Expression<Func<T, Q, bool>> _predicate;
        private readonly Func<Q, bool> _shouldIgnore;
        private readonly Func<Q, bool> _checkInvalid;

        public QueryRule(
            Expression<Func<T, Q, bool>> predicate,
            Expression<Func<Q, bool>> shouldIgnore,
            Expression<Func<Q, bool>> checkInvalid
        )
        {
            _predicate = predicate;
            _shouldIgnore = shouldIgnore.Compile();
            _checkInvalid = checkInvalid.Compile();
        }

        public bool CheckInvalid(Q query) => _checkInvalid(query);
        public bool ShouldIgnore(Q query) => _shouldIgnore(query);

        public Expression<Func<T, bool>> GetFilter(Q query)
        {
            return _predicate.Curry(query);
        }
    }
}
