using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using TheSSS.DicomViewer.Domain.Entities; // Required for seeding
using TheSSS.DicomViewer.Infrastructure.Data;
using FellowOakDicom; // Assuming FO-DICOM is used for DICOM parsing

namespace TheSSS.DicomViewer.IntegrationTests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly IConfiguration _configuration;
    private readonly DicomTestDatasetManager _datasetManager; // Made available for seeding methods
    private string _dbFilePath = string.Empty; // For file-based SQLite

    public string ConnectionString { get; private set; } = default!;

    // Static lock and flag to ensure migrations are applied only once per test run for a shared DB.
    // If each collection/class gets a new DB file, this might not be strictly needed for migration part
    // but can be useful for one-time global setup if any.
    private static readonly object _dbInitializationLock = new();
    private static bool _isDatabaseGloballyInitialized = false;


    public DatabaseFixture(IConfiguration configuration, DicomTestDatasetManager datasetManager)
    {
        _configuration = configuration;
        _datasetManager = datasetManager; // Store for potential use in seeding

        var configuredConnectionString = _configuration.GetConnectionString("TestDatabase");

        if (string.IsNullOrEmpty(configuredConnectionString) || configuredConnectionString.ToLowerInvariant().Contains(":memory:"))
        {
            // Use a unique in-memory database for each fixture instance if not explicitly file-based.
            // "DataSource=file:{Guid.NewGuid()}?mode=memory&cache=shared" ensures it's unique but kept alive by one connection.
            // However, for true isolation and migration testing, a unique file per test run/collection is better.
            // For simplicity with xUnit fixtures, a shared in-memory might require careful collection management.
            // Let's default to a unique file-based DB per fixture instance to support migrations robustly.
            _dbFilePath = Path.Combine(Path.GetTempPath(), $"testdb_{Guid.NewGuid()}.db");
            ConnectionString = $"Data Source={_dbFilePath}";
        }
        else
        {
            // If a specific file is configured, use it but be wary of parallel test runs if not unique.
            // For safety, append a GUID or use a unique path mechanism.
            // Here, we assume the configured string is a template or a base name.
            // For this example, we will use the configured path directly if it's not memory.
            // It's better if `appsettings.IntegrationTests.json` uses a placeholder like "Data Source=integration_test_{guid}.db"
            // or this fixture manages uniqueness.
            _dbFilePath = configuredConnectionString.Replace("Data Source=", ""); // simplify path extraction
             if (Path.GetDirectoryName(_dbFilePath) == string.Empty) // If only filename
             {
                _dbFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(_dbFilePath)); // Place in temp
             }
            ConnectionString = $"Data Source={_dbFilePath}";
        }
    }

    public async Task InitializeAsync()
    {
        // This lock ensures that for a given DatabaseFixture instance (shared within a collection),
        // the database creation and migration occurs only once.
        // If _isDatabaseGloballyInitialized is used for a truly shared DB (e.g. static connection string),
        // then it prevents re-migration across collections too.
        // For per-fixture file DB, the lock is more about the operations on *this* fixture's DB.

        lock (_dbInitializationLock) // Lock on a static object for global init control
        {
            // if (_isDatabaseGloballyInitialized && ConnectionString.Contains(":memory:")) return; // Example for shared in-mem

            using var context = CreateContextInternal(true); // Use internal to bypass logging if any
            // Ensure any previous instance of the file DB is removed for a clean slate
            if (!ConnectionString.Contains(":memory:") && File.Exists(_dbFilePath))
            {
                context.Database.EnsureDeleted();
            }
            context.Database.Migrate(); // Apply EF Core migrations

            // _isDatabaseGloballyInitialized = true; // Set if DB is truly globally shared
        }
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Clean up the database file if one was created by this fixture
        if (!string.IsNullOrEmpty(_dbFilePath) && File.Exists(_dbFilePath) && !ConnectionString.Contains(":memory:"))
        {
            // Attempt to delete the database file.
            // This might require ensuring all connections are closed.
            // Forcibly clear pools related to the connection string if using pooling with SQLite.
            // Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools(); // Might be too broad
            try
            {
                // A final context to ensure deletion.
                // Create a new options instance for disposal context.
                var optionsBuilder = new DbContextOptionsBuilder<DicomDbContext>();
                optionsBuilder.UseSqlite(ConnectionString);
                await using (var context = new DicomDbContext(optionsBuilder.Options))
                {
                    await context.Database.EnsureDeletedAsync();
                }
                // Fallback, though EnsureDeletedAsync should handle it.
                // File.Delete(_dbFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete test database '{_dbFilePath}': {ex.Message}");
                // Log this error appropriately in a real test suite
            }
        }
    }
    private DicomDbContext CreateContextInternal(bool useMinimalLogging = false)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DicomDbContext>();
        optionsBuilder.UseSqlite(ConnectionString);
        if (useMinimalLogging)
        {
            // Optionally configure minimal logging or no logging for fixture setup
        }
        else
        {
            // Optionally configure more detailed logging for tests
            // optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            // optionsBuilder.EnableSensitiveDataLogging();
        }
        return new DicomDbContext(optionsBuilder.Options);
    }
    public DicomDbContext CreateContext()
    {
       return CreateContextInternal(false);
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

    public async Task SeedMetadataFromDicomFilesAsync(string datasetName, int maxFilesToProcess = int.MaxValue)
    {
        var filePaths = _datasetManager.GetAllFilePathsFromDataset(datasetName).Take(maxFilesToProcess);
        await using var context = CreateContext();

        foreach (var filePath in filePaths)
        {
            try
            {
                var dicomFile = await DicomFile.OpenAsync(filePath);
                var dataset = dicomFile.Dataset;

                string patientId = dataset.GetSingleValueOrDefault(DicomTag.PatientID, $"PAT_{Guid.NewGuid().ToString().Substring(0, 8)}");
                string patientName = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Unknown^Patient");

                var patient = await context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
                if (patient == null)
                {
                    patient = new Patient
                    {
                        PatientId = patientId,
                        PatientName = patientName,
                        PatientBirthDate = dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, string.Empty),
                        PatientSex = dataset.GetSingleValueOrDefault(DicomTag.PatientSex, string.Empty),
                        LastUpdateTime = DateTime.UtcNow
                    };
                    context.Patients.Add(patient);
                    await context.SaveChangesAsync(); // Save patient to get DB-generated ID if applicable
                }

                string studyInstanceUid = dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
                var study = await context.Studies.FirstOrDefaultAsync(s => s.StudyInstanceUid == studyInstanceUid);
                if (study == null)
                {
                    study = new Study
                    {
                        StudyInstanceUid = studyInstanceUid,
                        PatientId = patient.Id, // Assuming Patient.Id is the PK
                        StudyDate = dataset.GetDateTime(DicomTag.StudyDate, DicomTag.StudyTime).GetValueOrDefault(),
                        AccessionNumber = dataset.GetSingleValueOrDefault(DicomTag.AccessionNumber, string.Empty),
                        StudyDescription = dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, string.Empty),
                        LastUpdateTime = DateTime.UtcNow
                    };
                    context.Studies.Add(study);
                    await context.SaveChangesAsync();
                }

                string seriesInstanceUid = dataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
                var series = await context.Series.FirstOrDefaultAsync(s => s.SeriesInstanceUid == seriesInstanceUid);
                if (series == null)
                {
                    series = new Series
                    {
                        SeriesInstanceUid = seriesInstanceUid,
                        StudyId = study.Id, // Assuming Study.Id is the PK
                        Modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, string.Empty),
                        SeriesNumber = dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, string.Empty),
                        SeriesDescription = dataset.GetSingleValueOrDefault(DicomTag.SeriesDescription, string.Empty),
                        LastUpdateTime = DateTime.UtcNow
                    };
                    context.Series.Add(series);
                    await context.SaveChangesAsync();
                }

                string sopInstanceUid = dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
                var instance = await context.Instances.FirstOrDefaultAsync(i => i.SopInstanceUid == sopInstanceUid);
                if (instance == null)
                {
                    instance = new Instance
                    {
                        SopInstanceUid = sopInstanceUid,
                        SeriesId = series.Id, // Assuming Series.Id is the PK
                        SopClassUid = dataset.GetSingleValue<string>(DicomTag.SOPClassUID),
                        InstanceNumber = dataset.GetSingleValueOrDefault(DicomTag.InstanceNumber, string.Empty),
                        InstanceFilePath = filePath, // Store path to the original file
                        PhotometricInterpretation = dataset.GetSingleValueOrDefault(DicomTag.PhotometricInterpretation, string.Empty),
                        Rows = dataset.GetSingleValueOrDefault(DicomTag.Rows, (ushort)0),
                        Columns = dataset.GetSingleValueOrDefault(DicomTag.Columns, (ushort)0),
                        IsAnonymized = false, // Default
                        LastUpdateTime = DateTime.UtcNow
                    };
                    context.Instances.Add(instance);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing DICOM file {filePath} for seeding: {ex.Message}");
            }
        }
        await context.SaveChangesAsync();
    }
}