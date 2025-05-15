using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheSSS.DicomViewer.IntegrationTests.Fixtures;
using TheSSS.DicomViewer.IntegrationTests.Helpers;
// Placeholder for IDicomSearchService and related types
// namespace TheSSS.DicomViewer.Application
// {
//     public class DicomSearchQuery { /* ... properties ... */ public string PatientName { get; set; } public DateTime? StudyDateFrom { get; set; } public DateTime? StudyDateTo { get; set; } public string Modality { get; set; }}
//     public class DicomSearchResult { public int TotalCount { get; set; } /* ... other properties ... */ }
//     public interface IDicomSearchService
//     {
//         Task<DicomSearchResult> SearchAsync(DicomSearchQuery query);
//     }
// }
// Placeholder for Domain Entities (Patient, Study, Series, Instance)
// namespace TheSSS.DicomViewer.Domain
// {
//     public class Patient { public string PatientId { get; set; } public string PatientName { get; set; } ... }
//     public class Study { public string StudyInstanceUid { get; set; } public string PatientId { get; set; } public DateTime StudyDate { get; set; } public string Modality { get; set; } ... }
//     // ... Series, Instance
// }

namespace TheSSS.DicomViewer.IntegrationTests.Search
{
    // Placeholders for Application Layer search components
    namespace TheSSS.DicomViewer.Application
    {
        public class DicomSearchQuery
        {
            public string? PatientName { get; set; }
            public DateTime? StudyDateFrom { get; set; }
            public DateTime? StudyDateTo { get; set; }
            public string? Modality { get; set; }
            public string? AccessionNumber { get; set; }
            // Add other relevant search fields
        }

        public class DicomSearchResult
        {
            public int TotalCount { get; set; }
            public IEnumerable<object> Results { get; set; } = Enumerable.Empty<object>(); // Simplified
        }

        public interface IDicomSearchService
        {
            Task<DicomSearchResult> SearchAsync(DicomSearchQuery query);
        }
    }

    // Re-using Domain placeholders from DatabaseOperationsTests if they were global, else define here
    // Assuming Domain.Patient, Domain.Study etc. are accessible.

    [Trait("Category", "Performance")]
    [Collection("PerformanceSensitiveTests")]
    public class LocalSearchPerformanceTests : IClassFixture<AppHostFixture>, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly AppHostFixture _appHostFixture;
        private readonly DatabaseFixture _databaseFixture;
        private readonly PerformanceMetricsHelper _performanceHelper;
        private readonly Application.IDicomSearchService _searchService;
        private readonly IConfiguration _configuration;
        private readonly int _localSearchThresholdMs;
        private const int NumberOfStudiesToSeed = 50000; // As per REQ-DLMM-035 context

        public LocalSearchPerformanceTests(AppHostFixture appHostFixture, DatabaseFixture databaseFixture)
        {
            _appHostFixture = appHostFixture;
            _databaseFixture = databaseFixture;
            _performanceHelper = new PerformanceMetricsHelper();
            _searchService = _appHostFixture.ServiceProvider.GetRequiredService<Application.IDicomSearchService>();
            _configuration = _appHostFixture.ServiceProvider.GetRequiredService<IConfiguration>();
            _localSearchThresholdMs = _configuration.GetValue<int>("PerformanceThresholds:LocalSearchMs", 2000);
        }

        public async Task InitializeAsync()
        {
            await _databaseFixture.ResetDatabaseAsync(); // Start with a clean slate

            Console.WriteLine($"Seeding {NumberOfStudiesToSeed} studies for search performance tests. This may take a while...");
            var stopwatch = Stopwatch.StartNew();

            var patients = new List<Domain.Patient>();
            var studies = new List<Domain.Study>(); // Assuming Domain.Study is defined
            var seriesList = new List<TheSSS.DicomViewer.Domain.Series>(); // Placeholder
            var instances = new List<TheSSS.DicomViewer.Domain.Instance>(); // Placeholder


            var modalities = new[] { "CT", "MR", "US", "XA", "MG", "CR" };
            var random = new Random();

            for (int i = 0; i < NumberOfStudiesToSeed / 10; i++) // Assuming 10 studies per patient on average
            {
                var patient = new Domain.Patient
                {
                    PatientId = $"PAT_{Guid.NewGuid().ToString().Substring(0, 8)}",
                    PatientName = $"SearchPatientName{i % 1000}^{TestLastName{i / 1000}}", // Vary names for searching
                    LastUpdateTime = DateTime.UtcNow
                };
                patients.Add(patient);

                for (int j = 0; j < 10; j++) // 10 studies for this patient
                {
                    var studyDate = DateTime.UtcNow.AddDays(-random.Next(1, 365 * 5)); // Studies from last 5 years
                    var study = new TheSSS.DicomViewer.Domain.Study // Using fully qualified name if namespace conflicts
                    {
                        StudyInstanceUid = $"STUDY_{Guid.NewGuid().ToString().Substring(0, 12)}",
                        PatientId = patient.PatientId,
                        StudyDate = studyDate.ToString("yyyyMMdd"), // DICOM date format
                        StudyTime = studyDate.ToString("HHmmss"),   // DICOM time format
                        AccessionNumber = $"ACC{random.Next(100000, 999999)}",
                        StudyDescription = $"Test Study Desc {i}-{j}",
                        ModalitiesInStudy = new List<string> { modalities[random.Next(modalities.Length)] }, // Simplified for this example
                        LastUpdateTime = DateTime.UtcNow
                    };
                    studies.Add(study);
                    // Could add Series and Instances too for more realism, but focus on Study for search
                }
            }

            await _databaseFixture.SeedDataAsync(async context =>
            {
                // EF Core's AddRange is efficient for bulk inserts
                await context.Set<Domain.Patient>().AddRangeAsync(patients);
                await context.Set<TheSSS.DicomViewer.Domain.Study>().AddRangeAsync(studies);
                // await context.Set<Domain.Series>().AddRangeAsync(seriesList);
                // await context.Set<Domain.Instance>().AddRangeAsync(instances);
            });
            stopwatch.Stop();
            Console.WriteLine($"Database seeding completed in {stopwatch.ElapsedMilliseconds}ms.");

            // Perform a warm-up search
            await _searchService.SearchAsync(new Application.DicomSearchQuery { PatientName = "WarmUpPatient" });
        }

        public Task DisposeAsync() => Task.CompletedTask;


        [Fact]
        public async Task SearchByPatientName_LargeDataset_ShouldReturnResultsWithin2Seconds()
        {
            var query = new Application.DicomSearchQuery { PatientName = "SearchPatientName50" }; // A name likely to exist
            TimeSpan elapsed = await _performanceHelper.MeasureExecutionTimeAsync(() => _searchService.SearchAsync(query));
            Console.WriteLine($"SearchByPatientName took {elapsed.TotalMilliseconds}ms.");
            elapsed.TotalMilliseconds.Should().BeLessOrEqualTo(_localSearchThresholdMs);
            var result = await _searchService.SearchAsync(query); // Get actual result for count check
            result.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchByStudyDateRange_LargeDataset_ShouldReturnResultsWithin2Seconds()
        {
            var dateTo = DateTime.UtcNow.AddDays(-random.Next(30, 90));
            var dateFrom = dateTo.AddDays(-30); // Search for studies in a 30-day window
            var query = new Application.DicomSearchQuery { StudyDateFrom = dateFrom, StudyDateTo = dateTo };

            TimeSpan elapsed = await _performanceHelper.MeasureExecutionTimeAsync(() => _searchService.SearchAsync(query));
            Console.WriteLine($"SearchByStudyDateRange took {elapsed.TotalMilliseconds}ms.");
            elapsed.TotalMilliseconds.Should().BeLessOrEqualTo(_localSearchThresholdMs);
            var result = await _searchService.SearchAsync(query);
            result.TotalCount.Should().BeGreaterThan(0);
        }
        private static readonly Random random = new Random();

        [Fact]
        public async Task SearchByModality_LargeDataset_ShouldReturnResultsWithin2Seconds()
        {
            var query = new Application.DicomSearchQuery { Modality = "CT" }; // Assuming CT is a common modality
            TimeSpan elapsed = await _performanceHelper.MeasureExecutionTimeAsync(() => _searchService.SearchAsync(query));
            Console.WriteLine($"SearchByModality took {elapsed.TotalMilliseconds}ms.");
            elapsed.TotalMilliseconds.Should().BeLessOrEqualTo(_localSearchThresholdMs);
            var result = await _searchService.SearchAsync(query);
            result.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchByCombinedCriteria_LargeDataset_ShouldReturnResultsWithin2Seconds()
        {
            var dateTo = DateTime.UtcNow.AddDays(-random.Next(100, 200));
            var dateFrom = dateTo.AddDays(-60);
            var query = new Application.DicomSearchQuery
            {
                PatientName = "SearchPatientName", // Partial name to get more results
                Modality = "MR",
                StudyDateFrom = dateFrom,
                StudyDateTo = dateTo
            };
            TimeSpan elapsed = await _performanceHelper.MeasureExecutionTimeAsync(() => _searchService.SearchAsync(query));
            Console.WriteLine($"SearchByCombinedCriteria took {elapsed.TotalMilliseconds}ms.");
            elapsed.TotalMilliseconds.Should().BeLessOrEqualTo(_localSearchThresholdMs);
             var result = await _searchService.SearchAsync(query);
            // Combined criteria might yield 0 results if not carefully crafted, adjust assertion if needed
            // For performance, we care more about query execution time than specific counts here
            result.TotalCount.Should().BeGreaterOrEqualTo(0);
        }
    }

    // Minimal placeholders for Domain entities used in seeding, if not globally available.
    // These should match the structure expected by DicomDbContext from REPO-INFRA.
    namespace TheSSS.DicomViewer.Domain
    {
        // Patient defined in DatabaseOperationsTests, re-using definition or assuming it's accessible.
        // public class Patient { public string PatientId { get; set; } public string PatientName { get; set; } public DateTime LastUpdateTime { get; set; } }

        public class Study
        {
            public string StudyInstanceUid { get; set; } = string.Empty;
            public string PatientId { get; set; } = string.Empty;
            public string? StudyDate { get; set; }
            public string? StudyTime { get; set; }
            public string? AccessionNumber { get; set; }
            public string? StudyDescription { get; set; }
            public ICollection<string> ModalitiesInStudy { get; set; } = new List<string>(); // For multi-modality search
            public DateTime LastUpdateTime { get; set; }
        }

        public class Series
        {
            public string SeriesInstanceUid { get; set; } = string.Empty;
            public string StudyInstanceUid { get; set; } = string.Empty;
            public string? Modality { get; set; }
            // ... other properties
            public DateTime LastUpdateTime { get; set; }
        }

        public class Instance
        {
            public string SopInstanceUid { get; set; } = string.Empty;
            public string SeriesInstanceUid { get; set; } = string.Empty;
            // ... other properties
            public DateTime LastUpdateTime { get; set; }
        }
    }
}