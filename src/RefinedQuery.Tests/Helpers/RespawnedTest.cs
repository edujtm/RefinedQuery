using System.Threading.Tasks;

using Respawn;
using Xunit;

using RefinedQuery.Tests.Helpers.Xunit;

namespace RefinedQuery.Tests.Helpers
{
    public class RespawnedTest : IAsyncLifetime
    {
        public DbContextFixture Fixture { get; }

        private readonly Checkpoint _checkpoint;

        public RespawnedTest(DbContextFixture fixture)
        {
            Fixture = fixture; 

            _checkpoint = new Checkpoint()
            {
                WithReseed = true
            };
        }

        public Task InitializeAsync() => _checkpoint.Reset(Fixture.ConnectionString);

        public Task DisposeAsync() => Task.CompletedTask;
    }
}