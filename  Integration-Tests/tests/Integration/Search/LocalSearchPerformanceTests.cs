using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TheSSS.DicomViewer.Application.Services; // For IDicomSearchService, SearchCriteria, StudySearchResult
using TheSSS.DicomViewer.Domain.Entities;    // For Patient, Study, Series, Instance
using TheSSS.DicomViewer.Infrastructure.Data; // For DicomDbContext
using TheSSS.DicomViewer.IntegrationTests.Fixtures;
using TheSSS.DicomViewer.IntegrationTests.Helpers;

namespace TheSSS.DicomViewer.IntegrationTests.Search;

[Trait("Category", "Performance")]
[Collection("PerformanceSensitiveTests")]
public class LocalSearchPerformanceTests : IClassFixture<AppHostFixture>, IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly AppHostFixture _appHostFixture;
    private readonly DatabaseFixture _dbFixture;
    private readonly IDicomSearchService _searchService;
    private readonly TimeSpan _searchThreshold;
    private readonly IConfiguration _configuration;

    private static bool _isDatabaseSeededForSearchPerf = false;
    private static readonly object _seedLock = new object();

    public LocalSearchPerformanceTests(AppHostFixture appHostFixture, DatabaseFixture dbFixture)
    {
        _appHostFixture = appHostFixture;
        _dbFixture = dbFixture;
        _configuration = _appHostFixture.ServiceProvider.GetRequiredService<IConfiguration>();
        _searchService = _appHostFixture.ServiceProvider.GetRequiredService<IDicomSearchService>();
        
        var thresholdMs = _configuration.GetValue<int>("PerformanceThresholds:LocalSearchMs", 2000);
        _searchThreshold = TimeSpan.FromMilliseconds(thresholdMs);
    }

    public async Task InitializeAsync()
    {
        // One-time seeding for this test class/collection
        bool seedDatabase = false;
        lock (_seedLock)
        {
            if (!_isDatabaseSeededForSearchPerf)
            {
                seedDatabase = true;
                _isDatabaseSeededForSearchPerf = true; // Mark as seeded even if it fails, to prevent re-attempts in one run
            }
        }

        if (seedDatabase)
        {
            Debug.WriteLine("Seeding database for LocalSearchPerformanceTests...");
            var targetStudyCount = _configuration.GetValue<int>("PerformanceTestData:SearchStudyCount", 50000);
            await SeedLargeDatabaseAsync(targetStudyCount);
            Debug.WriteLine($"Database seeding complete for {targetStudyCount} studies.");
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task SeedLargeDatabaseAsync(int targetStudyCount)
    {
        await _dbFixture.ResetDatabaseAsync(); // Clear before seeding

        var random = new Random(12345); // Fixed seed for reproducibility
        var baseDate = new DateTime(2020, 1, 1);
        
        string[] patientNameBases = { "SMITH", "JOHNSON", "WILLIAMS", "BROWN", "JONES", "GARCIA", "MILLER", "DAVIS", "RODRIGUEZ", "MARTINEZ" };
        string[] patientFirstNames = { "JAMES", "MARY", "ROBERT", "PATRICIA", "JOHN", "JENNIFER", "MICHAEL", "LINDA", "DAVID", "ELIZABETH" };
        string[] modalities = { "CT", "MR", "US", "XA", "CR", "DX", "MG", "NM", "PT", "OT" };
        string[] studyDescriptions = { "CHEST PA", "BRAIN W/O CONTRAST", "ABDOMEN COMPLETE", "KNEE RT", "LUMBAR SPINE", "PELVIS AP", "CAROTID DOPPLER", "CARDIAC ECHO", "MAMMOGRAM SCREENING", "PET BRAIN FDG" };

        var patients = new List<Patient>();
        int numPatients = Math.Max(1, targetStudyCount / 25); // Aim for ~25 studies per patient on average
        for (int i = 0; i < numPatients; i++)
        {
            patients.Add(new Patient
            {
                PatientId = $"PAT{i:D7}",
                PatientName = $"{patientNameBases[random.Next(patientNameBases.Length)]}^{patientFirstNames[random.Next(patientFirstNames.Length)]}_{i:D4}",
                PatientBirthDate = baseDate.AddDays(-random.Next(15000, 30000)).ToString("yyyyMMdd"), // Approx 40-80 years old
                PatientSex = random.Next(0, 2) == 0 ? "M" : "F"
            });
        }
        await _dbFixture.SeedDataAsync(ctx => ctx.Patients.AddRange(patients));

        // Re-fetch patients with IDs if DB generates them, or assume PatientId is the PK
        // For this example, we assume PatientId is text PK and set by us.
        var allPatientsInDb = patients; // If IDs were auto-generated, re-query: await _dbFixture.CreateContext().Patients.ToListAsync();


        var studies = new List<Study>();
        for (int i = 0; i < targetStudyCount; i++)
        {
            var patient = allPatientsInDb[random.Next(allPatientsInDb.Count)];
            studies.Add(new Study
            {
                StudyInstanceUid = $"1.2.840.113619.2.400.3.111222333.444.5555555555.{i:D7}", // Generate unique UIDs
                PatientId = patient.Id, // FK to Patient
                StudyDate = baseDate.AddDays(random.Next(0, 1460)).ToString("yyyyMMdd"), // Studies over 4 years
                StudyTime = $"{random.Next(0,24):D2}{random.Next(0,60):D2}{random.Next(0,60):D2}",
                AccessionNumber = $"ACC{i:D7}",
                StudyDescription = studyDescriptions[random.Next(studyDescriptions.Length)]
                // Modalities are typically per-series, but we can add a primary modality here for search convenience
                // Or use a StudyModality junction table as per schema. For this seed, keep it simple.
            });
        }
        await _dbFixture.SeedDataAsync(ctx => ctx.Studies.AddRange(studies));
        
        // Could also seed Series and Instances for more complex search scenarios, but Studies are primary for REQ-DLMM-035.
        // For instance-level searches, more detailed seeding would be needed.
    }


    private async Task PerformSearchTest(SearchCriteria criteria, string testDescription)
    {
        var elapsed = await PerformanceMetricsHelper.MeasureExecutionTimeAsync(async () =>
        {
            var results = await _searchService.SearchStudiesAsync(criteria); // Assuming SearchStudiesAsync
            results.Should().NotBeNull();
            // Depending on the search term, results might be empty, which is valid.
            // For performance, we care more about time than if specific results are found, assuming DB is seeded.
            // However, for specific known-data searches, you'd assert result count.
        });
        elapsed.Should().BeLessThanOrEqualTo(_searchThreshold, 
            $"{testDescription} took {elapsed.TotalMilliseconds:F0}ms, exceeding threshold of {_searchThreshold.TotalMilliseconds:F0}ms.");
    }

    [Fact]
    [Trait("Requirement", "REQ-DLMM-035")]
    [Trait("Requirement", "REQ-DLMM-017")] // General search
    [Trait("Requirement", "REQ-DLMM-021")] // Search by tags
    public async Task SearchByPatientName_LargeDataset_ShouldReturnResultsWithin2Seconds()
    {
        // Using a common base name that should match multiple seeded patients
        await PerformSearchTest(new SearchCriteria { PatientName = "SMITH*" }, "Search by Patient Name (wildcard)");
    }

    [Fact]
    [Trait("Requirement", "REQ-DLMM-035")]
    [Trait("Requirement", "REQ-DLMM-021")]
    public async Task SearchByStudyDateRange_LargeDataset_ShouldReturnResultsWithin2Seconds()
    {
        var dateFrom = new DateTime(2021, 1, 1);
        var dateTo = new DateTime(2021, 12, 31);
        await PerformSearchTest(new SearchCriteria { StudyDateFrom = dateFrom, StudyDateTo = dateTo }, "Search by Study Date Range");
    }
    
    [Fact]
    [Trait("Requirement", "REQ-DLMM-035")]
    [Trait("Requirement", "REQ-DLMM-021")]
    public async Task SearchByModality_LargeDataset_ShouldReturnResultsWithin2Seconds()
    {
         // This search would be more effective if Study table had a Modalities column or Series were seeded and searched.
         // For now, assuming SearchStudiesAsync can search across series modalities or a denormalized Study.PrimaryModality.
         // If not, this test would need adjustment based on actual search capabilities.
         // We'll simulate searching for a common modality like CT. If Series are not seeded/joined, this might be slow or return 0.
        await PerformSearchTest(new SearchCriteria { Modalities = new List<string> { "CT" } }, "Search by Modality (CT)");
    }

    [Fact]
    [Trait("Requirement", "REQ-DLMM-035")]
    [Trait("Requirement", "REQ-DLMM-021")]
    public async Task SearchByCombinedCriteria_LargeDataset_ShouldReturnResultsWithin2Seconds()
    {
        var dateFrom = new DateTime(2022, 1, 1);
        await PerformSearchTest(new SearchCriteria { PatientName = "JONES*", StudyDateFrom = dateFrom, Modalities = new List<string> { "MR" } }, "Search by Combined Criteria");
    }
}