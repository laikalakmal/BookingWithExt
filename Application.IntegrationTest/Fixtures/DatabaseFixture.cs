using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTest.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        public AppDbContext DbContext { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }

        public DatabaseFixture()
        {
            // Setup service collection
            var services = new ServiceCollection();

            // Use in-memory database for testing
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("ProductServiceTestDb_" + Guid.NewGuid().ToString()));

            // Build the service provider
            ServiceProvider = services.BuildServiceProvider();

            // Get the DbContext
            DbContext = ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure database is created
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }
    }
}