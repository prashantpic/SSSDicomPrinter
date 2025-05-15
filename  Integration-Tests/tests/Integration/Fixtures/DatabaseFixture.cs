using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Infrastructure.Data; // Namespace for DicomDbContext

namespace TheSSS.DicomViewer.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IAsyncLifetime
    {
        public string ConnectionString { get; private set; }
        private readonly IConfiguration _configuration;
        private static readonly object _lock = new object();
        private static bool _databaseInitialized = false;

        public DatabaseFixture(IConfiguration configuration)
        {
            _configuration = configuration;
            // Prefer a unique file-based database for each DatabaseFixture instance to ensure full isolation
            // if tests might run in parallel or if in-memory shared cache behavior is not sufficient.
            // For this example, using the configured string, which could be file-based or in-memory.
            var configuredConnectionString = _configuration.GetConnectionString("TestDatabase");
            if (string.IsNullOrEmpty(configuredConnectionString) || configuredConnectionString.ToLowerInvariant().Contains(":memory:"))
            {
                // Ensure unique in-memory DB per fixture instance if using :memory: without shared cache for true isolation
                // However, for shared :memory: with cache for a collection, this is fine.
                // Using "DataSource=:memory:" without "cache=shared" means each connection is a new DB.
                // Using "DataSource=file::memory:?cache=shared" + a static lock for initialization is a common pattern for shared in-memory DB for a collection.
                ConnectionString = "DataSource=file::memory:?cache=shared"; // Requires careful handling of connection lifetime
            }
            else
            {
                // For file-based, ensure unique DB name if multiple fixtures could run in parallel from different collections.
                // Or rely on collection serialization.
                var dbFileName = Path.GetFileName(new Uri(configuredConnectionString.Replace("Data Source=", "")).LocalPath);
                var dbPath = Path.GetDirectoryName(new Uri(configuredConnectionString.Replace("Data Source=", "")).LocalPath) ?? Directory.GetCurrentDirectory();
                // Ensure a unique DB for this fixture instance if parallel collections might use this fixture type
                // string uniqueDbName = $"{Path.GetFileNameWithoutExtension(dbFileName)}_{Guid.NewGuid()}{Path.GetExtension(dbFileName)}";
                // ConnectionString = $"Data Source={Path.Combine(dbPath, uniqueDbName)}";
                ConnectionString = configuredConnectionString; // Assume config provides a suitable unique path or tests are serialized
            }
        }

        public async Task InitializeAsync()
        {
            // For shared in-memory (cache=shared) or file-based DB used by a collection,
            // migrations should run once.
            if (ConnectionString.Contains("cache=shared") || !ConnectionString.Contains(":memory:"))
            {
                lock (_lock)
                {
                    if (!_databaseInitialized)
                    {
                        using (var context = CreateContextInternal())
                        {
                            context.Database.EnsureDeleted(); // Clean slate
                            context.Database.Migrate();    // Apply migrations
                        }
                        _databaseInitialized = true;
                    }
                }
            }
            else // For non-shared in-memory DB (each CreateContext is a new DB)
            {
                using (var context = CreateContextInternal())
                {
                    // context.Database.EnsureDeleted(); // Not needed, it's a new DB
                    await context.Database.MigrateAsync(); // Apply migrations
                }
            }
        }

        public async Task DisposeAsync()
        {
            if (!ConnectionString.Contains(":memory:") && !ConnectionString.Contains("cache=shared")) // Only delete unique file DBs
            {
                // If a unique file DB was created per fixture, delete it.
                // This requires careful handling as the file might be locked.
                // Often, EnsureDeleted in InitializeAsync (for the *next* run) is safer.
                try
                {
                    using (var context = CreateContextInternal())
                    {
                        await context.Database.EnsureDeletedAsync();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to delete test database {ConnectionString}: {ex.Message}");
                }
            }
            // For shared in-memory (cache=shared), the DB vanishes when all connections are closed.
            // Resetting _databaseInitialized might be needed if tests run in multiple batches in the same process.
        }
        
        private DicomDbContext CreateContextInternal()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DicomDbContext>();
            optionsBuilder.UseSqlite(ConnectionString);
            return new DicomDbContext(optionsBuilder.Options);
        }

        public DicomDbContext CreateContext()
        {
            return CreateContextInternal();
        }

        public async Task SeedDataAsync(Action<DicomDbContext> seedAction)
        {
            await using var context = CreateContext();
            seedAction(context);
            await context.SaveChangesAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            await using var context = CreateContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
        }
    }
}