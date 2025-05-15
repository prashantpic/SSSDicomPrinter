using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheSSS.DicomViewer.Application.Services; // Assumed namespace for app services
using TheSSS.DicomViewer.Infrastructure.Data;   // Assumed namespace for DbContext
using TheSSS.DicomViewer.IntegrationTests.Mocks;
using TheSSS.DicomViewer.IntegrationTests.Helpers;
using TheSSS.DicomViewer.Presentation.ViewModels; // Assumed namespace for ViewModels
// Define placeholder interfaces if they are not in referenced projects
// These should ideally come from REPO-APP-SERVICES or REPO-COMMON
#if !PROJECT_REFERENCES_DEFINE_THESE_INTERFACES
namespace TheSSS.DicomViewer.Application.Services
{
    public interface ISystemMonitoringOrchestrationService { Task CheckSystemStatusAsync(); /* other methods */ }
    public interface IApplicationUpdateService { Task<Version?> CheckForUpdatesAsync(); /* other methods */ }
    public interface IStorageMonitorService { Task<double> GetFreeSpacePercentageAsync(string path); }
    public class PacsNodeStatus { public string NodeName { get; set; } = string.Empty; public bool IsConnected { get; set; } }
    public interface IPacsConnectivityMonitorService { Task<List<PacsNodeStatus>> CheckPacsNodesAsync(); }
    public interface IDicomFileProcessor { /* Methods to process DICOM files */ }
}
#endif

namespace TheSSS.DicomViewer.IntegrationTests.Fixtures;

public class AppHostFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;
    private ServiceProvider? _serviceProviderInternal;
    public IConfiguration Configuration { get; private set; } = default!;

    // Expose specific mock instances if tests need to configure them directly
    public Mock<IApplicationUpdateService> MockApplicationUpdateService { get; } = new();
    public Mock<IStorageMonitorService> MockStorageMonitorService { get; } = new();
    public Mock<IPacsConnectivityMonitorService> MockPacsConnectivityMonitorService { get; } = new();

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.IntegrationTests.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(Configuration);

        // Register application core services (concrete types from REPO-APP-SERVICES)
        services.AddScoped<ILicensingOrchestrationService, LicensingOrchestrationService>();
        services.AddScoped<IDatabaseAdministrationService, DatabaseAdministrationService>();
        services.AddScoped<IDicomSearchService, DicomSearchService>();
        services.AddScoped<ISystemMonitoringOrchestrationService, SystemMonitoringOrchestrationService>();
        // Assuming IDicomFileProcessor is needed by DatabaseFixture seeding, register it.
        // If it's a real service, register it. If it's a utility, it might not be DI managed.
        // services.AddScoped<IDicomFileProcessor, DicomFileProcessor>();


        // Register external service clients or their mocks
        if (Configuration.GetValue<bool>("MockSettings:EnableOdooLicensingMock", true))
        {
            services.AddSingleton<ILicensingApiClient, MockOdooLicensingApiClient>();
        }
        else
        {
            // services.AddSingleton<ILicensingApiClient, OdooLicensingApiClient>(); // Actual implementation
            throw new NotSupportedException("Actual Odoo Licensing API client not configured for integration tests.");
        }

        if (Configuration.GetValue<bool>("MockSettings:EnableSmtpMock", true))
        {
            services.AddSingleton<ISmtpService, MockSmtpService>();
        }
        else
        {
            // services.AddSingleton<ISmtpService, RealSmtpService>(); // Actual implementation
            throw new NotSupportedException("Actual SMTP service not configured for integration tests.");
        }

        // Register other mocked services using Moq directly
        services.AddSingleton(MockApplicationUpdateService.Object); // Register the mocked object
        services.AddSingleton(MockStorageMonitorService.Object);
        services.AddSingleton(MockPacsConnectivityMonitorService.Object);


        // Register Infrastructure implementations (e.g., Repositories)
        // DbContext itself is usually managed by DatabaseFixture or per-test, not as a singleton/scoped here unless specific design.
        // Repositories would typically be scoped if DbContext is scoped.
        // Example:
        // services.AddScoped<IPatientRepository, PatientRepository>();

        // Register Test Helpers
        services.AddSingleton<DicomTestDatasetManager>(); // Relies on IConfiguration
        services.AddSingleton<PerformanceMetricsHelper>();
        services.AddSingleton<UiInteractionHelper>(); // Relies on IServiceProvider

        // Register ViewModels (typically Transient or Scoped depending on usage)
        services.AddTransient<DicomImageViewerViewModel>(); // Assuming this ViewModel exists and is used by tests

        _serviceProviderInternal = services.BuildServiceProvider();
        ServiceProvider = _serviceProviderInternal;

        // Allow UiInteractionHelper to resolve services itself
        var uiHelperInstance = ServiceProvider.GetRequiredService<UiInteractionHelper>();
        // uiHelperInstance.Initialize(ServiceProvider); // If UiInteractionHelper needs direct SP access after construction.

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_serviceProviderInternal is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            _serviceProviderInternal?.Dispose();
        }
        _serviceProviderInternal = null;
    }

    public T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}