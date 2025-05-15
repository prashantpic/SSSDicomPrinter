using Microsoft.EntityFrameworkCore;
using TheSSS.DicomViewer.Application.Services;
using TheSSS.DicomViewer.Domain.Entities; // Assuming Patient, Study entities are here
using TheSSS.DicomViewer.Infrastructure.Data; // For DicomDbContext
using TheSSS.DicomViewer.IntegrationTests.Fixtures;

namespace TheSSS.DicomViewer.IntegrationTests.Maintenance;

[Collection("SequentialIntegrationTests")]
public class DatabaseOperationsTests : IClassFixture<AppHostFixture>, IClassFixture<DatabaseFixture>
{
    private readonly AppHostFixture _appHostFixture;
    private readonly DatabaseFixture _dbFixture;
    private readonly IDatabaseAdministrationService _dbAdminService;

    public DatabaseOperationsTests(AppHostFixture appHostFixture, DatabaseFixture dbFixture)
    {
        _appHostFixture = appHostFixture;
        _dbFixture = dbFixture;
        _dbAdminService = _appHostFixture.ServiceProvider.GetRequiredService<IDatabaseAdministrationService>();
    }

    [Fact]
    [Trait("Category", "Maintenance")]
    [Trait("Requirement", "REQ-LDM-TST-001")] // Part of licensing/maintenance tests
    [Trait("Requirement", "REQ-DLMM-038")] // Database migration
    public async Task DatabaseMigration_OnFixtureInitialization_ShouldApplyAllPendingMigrations()
    {
        // Arrange: DatabaseFixture.InitializeAsync ensures migrations are applied.
        // Act & Assert: Verify by trying to interact with the database.
        await _dbFixture.ResetDatabaseAsync(); // Ensure a clean state for this check
        
        await using var context = _dbFixture.CreateContext();
        // A simple check: can we query a known table (e.g., Patients)?
        // This doesn't verify specific schema changes from a migration, but that the DB is usable.
        var canQueryPatients = async () => await context.Patients.AnyAsync();
        await canQueryPatients.Should().NotThrowAsync("Querying Patients table should not throw after migrations.");

        // To verify data preservation from a *previous* version, DatabaseFixture would need
        // a more complex setup to initialize an older schema, seed, then migrate.
        // The current test verifies migrations are applied to an empty DB by the fixture.
    }

    [Fact]
    [Trait("Category", "Maintenance")]
    [Trait("Requirement", "REQ-LDM-TST-001")]
    [Trait("Requirement", "REQ-DLMM-040")] // Backup database
    [Trait("Requirement", "REQ-7-022")] // Database operations
    public async Task BackupDatabase_ShouldCreateValidBackupFile()
    {
        // Arrange
        await _dbFixture.ResetDatabaseAsync();
        var initialPatient = new Patient { PatientId = "BACKUP_P001", PatientName = "Backup Test Patient" };
        await _dbFixture.SeedDataAsync(ctx =>
        {
            ctx.Patients.Add(initialPatient);
        });

        var backupDirectory = Path.Combine(Path.GetTempPath(), $"DicomViewerTestBackups_{Guid.NewGuid()}");
        Directory.CreateDirectory(backupDirectory);
        var backupFilePath = Path.Combine(backupDirectory, $"backup_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString().Substring(0,8)}.db");

        // Act
        var backupResult = await _dbAdminService.BackupDatabaseAsync(backupFilePath);

        // Assert
        backupResult.Should().BeTrue("Backup operation should report success.");
        File.Exists(backupFilePath).Should().BeTrue($"Backup file should be created at {backupFilePath}");
        new FileInfo(backupFilePath).Length.Should().BeGreaterThan(0, "Backup file should not be empty.");

        // Optional: Attempt to open the backup as a SQLite file to verify its integrity
        bool canOpenBackup = false;
        try
        {
            await using var backupContext = new DicomDbContext(new DbContextOptionsBuilder<DicomDbContext>().UseSqlite($"Data Source={backupFilePath}").Options);
            (await backupContext.Patients.CountAsync()).Should().Be(1, "Backup file should contain the seeded patient.");
            canOpenBackup = true;
        }
        catch (Exception ex)
        {
            // Log or output ex.Message for diagnostics
            Console.WriteLine($"Failed to open backup DB: {ex.Message}");
        }
        canOpenBackup.Should().BeTrue("Should be able to open the backup database file and query data.");

        // Cleanup
        if (File.Exists(backupFilePath)) File.Delete(backupFilePath);
        if (Directory.Exists(backupDirectory)) Directory.Delete(backupDirectory, true);
    }

    [Fact]
    [Trait("Category", "Maintenance")]
    [Trait("Requirement", "REQ-LDM-TST-001")]
    [Trait("Requirement", "REQ-DLMM-041")] // Restore database
    [Trait("Requirement", "REQ-7-022")] // Database operations
    public async Task RestoreDatabase_FromValidBackup_ShouldRestoreState()
    {
        // Arrange
        // 1. State A: Create DB, seed data, make a backup
        await _dbFixture.ResetDatabaseAsync();
        var patientInBackup = new Patient { PatientId = "RESTORE_P001", PatientName = "Patient In Backup" };
        await _dbFixture.SeedDataAsync(ctx => ctx.Patients.Add(patientInBackup));
        
        var backupFilePath = Path.Combine(Path.GetTempPath(), $"restore_test_backup_{Guid.NewGuid()}.db");
        await _dbAdminService.BackupDatabaseAsync(backupFilePath);
        File.Exists(backupFilePath).Should().BeTrue("Backup file for restore test must be created.");

        // 2. State B: Modify the current database (e.g., add/delete/change data)
        await using (var ctx = _dbFixture.CreateContext())
        {
            var originalPatient = await ctx.Patients.FindAsync(patientInBackup.Id);
            if(originalPatient != null) ctx.Patients.Remove(originalPatient);
            ctx.Patients.Add(new Patient { PatientId = "RESTORE_P002", PatientName = "Patient Before Restore" });
            await ctx.SaveChangesAsync();
        }

        // Verify current state is different from backup
        await using (var ctx = _dbFixture.CreateContext())
        {
            (await ctx.Patients.AnyAsync(p => p.PatientId == patientInBackup.PatientId)).Should().BeFalse("Original patient should be gone before restore.");
            (await ctx.Patients.AnyAsync(p => p.PatientId == "RESTORE_P002")).Should().BeTrue("New patient should exist before restore.");
        }

        // Act: Restore from the backup of State A
        var restoreResult = await _dbAdminService.RestoreDatabaseAsync(backupFilePath);

        // Assert
        restoreResult.Should().BeTrue("Restore operation should report success.");
        await using (var ctxAfterRestore = _dbFixture.CreateContext())
        {
            // Verify data matches State A
            (await ctxAfterRestore.Patients.CountAsync()).Should().Be(1, "Database should contain one patient after restore.");
            var restoredPatient = await ctxAfterRestore.Patients.FirstOrDefaultAsync();
            restoredPatient.Should().NotBeNull();
            restoredPatient!.PatientId.Should().Be(patientInBackup.PatientId);
            restoredPatient.PatientName.Should().Be(patientInBackup.PatientName);
            (await ctxAfterRestore.Patients.AnyAsync(p => p.PatientId == "RESTORE_P002")).Should().BeFalse("Patient from State B should be gone after restore.");
        }

        // Cleanup
        if (File.Exists(backupFilePath)) File.Delete(backupFilePath);
    }
}