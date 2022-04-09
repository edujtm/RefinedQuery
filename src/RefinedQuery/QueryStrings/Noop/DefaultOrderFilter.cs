
using RefinedQuery.Ordering;

namespace RefinedQuery.Noop
{
    /// <summary>
    /// No-op order filter.
    /// Used as an unconfigured ordering filter.
    /// </summary>
    internal class DefaultOrderFilter<Q> : AbstractOrderFilter<Q>
    {
    }
}