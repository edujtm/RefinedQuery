
using RefinedQuery.Querying;

namespace RefinedQuery.Noop
{
    /// <summary>
    /// No-op query filter.
    /// Used as an unconfigured query filter.
    /// </summary>
    internal class DefaultQueryFilter<T, Q> : AbstractQueryFilter<T, Q>
    {
    }
}