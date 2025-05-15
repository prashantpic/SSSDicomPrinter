using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Infrastructure; // For DicomDbContext

namespace TheSSS.DicomViewer.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IAsyncLifetime
    {
        public string ConnectionString { get; private set; }
        private readonly IConfiguration _configuration;
        private string _dbFilePath;

        // Constructor to receive IConfiguration, typically resolved from AppHostFixture or built ad-hoc
        public DatabaseFixture()
        {
            // This constructor is used when DatabaseFixture is injected directly.
            // For ICollectionFixture, xUnit creates an instance using a parameterless constructor if available,
            // or one that can be satisfied by other registered services if part of a more complex DI setup (not typical for xUnit fixtures directly).
            // AppHostFixture will build and provide IConfiguration. Tests can access it via AppHostFixture.
            // Here, we build a temporary one for standalone DatabaseFixture use or initial setup path.
            // In a combined collection, AppHostFixture.Configuration would be preferred.
             _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.IntegrationTests.json", optional: false, reloadOnChange: true)
                .Build();

            InitializeDatabasePath();
        }
        
        // This constructor would be used if IConfiguration is passed from AppHostFixture through a custom collection definition.
        // For simplicity with standard ICollectionFixture, the parameterless one is often relied upon.
        // public DatabaseFixture(IConfiguration configuration)
        // {
        //     _configuration = configuration;
        //     InitializeDatabasePath();
        // }

        private void InitializeDatabasePath()
        {
            var configuredConnectionString = _configuration.GetConnectionString("DicomDb");
            if (configuredConnectionString != null && configuredConnectionString.StartsWith("DataSource="))
            {
                 // Ensure unique DB for parallel runs if file-based and not in-memory
                var dbName = configuredConnectionString.Substring("DataSource=".Length);
                if (dbName.Equals(":memory:", StringComparison.OrdinalIgnoreCase)) {
                    ConnectionString = configuredConnectionString; // In-memory
                    _dbFilePath = null;
                } else {
                    _dbFilePath = Path.Combine(Path.GetTempPath(), $"test_dicom_{Guid.NewGuid()}.db"); // Unique DB file
                    ConnectionString = $"DataSource={_dbFilePath}";
                }
            }
            else
            {
                // Default to a unique temporary file-based SQLite DB if not configured or misconfigured
                _dbFilePath = Path.Combine(Path.GetTempPath(), $"test_dicom_{Guid.NewGuid()}.db");
                ConnectionString = $"DataSource={_dbFilePath}";
            }
        }


        public async Task InitializeAsync()
        {
            await using var context = CreateContext();
            await context.Database.EnsureDeletedAsync(); // Clean up any previous instance, if file-based
            await context.Database.MigrateAsync(); // Apply migrations

            // Optionally, seed a minimal baseline dataset if required for *all* database tests
            // await SeedBaselineDataAsync(context);
        }

        public async Task DisposeAsync()
        {
            if (_dbFilePath != null && File.Exists(_dbFilePath))
            {
                // Attempt to delete the database file.
                // EF Core connections might hold the file lock. Ensure all contexts are disposed.
                // For safety, a more robust cleanup might involve GC.Collect and GC.WaitForPendingFinalizers before delete.
                try
                {
                     // Ensure all connections are closed by disposing a final context.
                    await using (var context = CreateContext())
                    {
                        // The context itself doesn't need specific action here, just its disposal.
                    }
                    File.Delete(_dbFilePath);
                }
                catch (IOException)
                {
                    // Log or handle inability to delete, possibly due to file lock
                    Console.WriteLine($"Warning: Could not delete test database file: {_dbFilePath}");
                }
            }
        }

        public DicomDbContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DicomDbContext>();
            optionsBuilder.UseSqlite(ConnectionString);
            return new DicomDbContext(optionsBuilder.Options);
        }

        public async Task SeedDataAsync(Action<DicomDbContext> seedAction)
        {
            await using var context = CreateContext();
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                seedAction(context);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task SeedDataAsync<TEntity>(params TEntity[] entities) where TEntity : class
        {
            await using var context = CreateContext();
            context.Set<TEntity>().AddRange(entities);
            await context.SaveChangesAsync();
        }


        public async Task ResetDatabaseAsync()
        {
            await using var context = CreateContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
            // If baseline data is needed after reset:
            // await SeedBaselineDataAsync(context);
        }

        // Example baseline seeding, if needed
        // private async Task SeedBaselineDataAsync(DicomDbContext context)
        // {
        //     // Add any data that should be present for all tests using this fixture
        //     // For example, some default configuration entries or system users
        //     if (!await context.YourEntities.AnyAsync())
        //     {
        //         context.YourEntities.Add(new YourEntity { /* ... */ });
        //         await context.SaveChangesAsync();
        //     }
        // }
    }
}