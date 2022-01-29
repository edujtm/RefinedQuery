
using Xunit;

using RefinedQuery.Tests.Helpers;

namespace RefinedQuery.Tests.Helpers.Xunit
{

    [CollectionDefinition("DB", DisableParallelization = true)]
    public class DbCollection : ICollectionFixture<DbContextFixture>
    {
    }
}