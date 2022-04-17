
using RefinedQuery.Pagination;

namespace RefinedQuery.Noop
{
    /// <summary>
    /// No-op page filter.
    /// Used as an unconfigured page filter.
    /// </summary>
    internal class DefaultPageFilter<T, Q> : AbstractPageFilter<T, Q>
    {
    }
}