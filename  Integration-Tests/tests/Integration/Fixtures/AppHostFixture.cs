using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using TheSSS.DicomViewer.Application.Services; // Assuming this namespace for app services
using TheSSS.DicomViewer.Infrastructure.Services; // Assuming this namespace for infra services (like actual Odoo/SMTP client)
using TheSSS.DicomViewer.IntegrationTests.Mocks; // For MockOdooLicensingApiClient, MockSmtpService
using TheSSS.DicomViewer.Presentation.ViewModels; // If ViewModels are registered

namespace TheSSS.DicomViewer.IntegrationTests.Fixtures
{
    public class AppHostFixture : IAsyncLifetime
    {
        public IServiceProvider ServiceProvider { get; private set; } = default!;
        private ServiceCollection _services;
        private ServiceProvider? _builtProvider;

        public AppHostFixture()
        {
            _services = new ServiceCollection();
        }

        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.IntegrationTests.json", optional: false, reloadOnChange: true)
                .Build();

            _services.AddSingleton<IConfiguration>(configuration);

            // Register application services (examples, replace with actual services and interfaces)
            _services.AddScoped<ILicensingOrchestrationService, LicensingOrchestrationService>();
            _services.AddScoped<IDatabaseAdministrationService, DatabaseAdministrationService>();
            _services.AddScoped<IDicomSearchService, DicomSearchService>();
            _services.AddScoped<ISystemMonitoringOrchestrationService, SystemMonitoringOrchestrationService>(); // For AlertSystemTests
            _services.AddScoped<IApplicationUpdateService, ApplicationUpdateService>(); // For ApplicationUpdateSimulationTests, might be mocked

            // Register Infrastructure services (e.g., Repositories)
            // DbContext is usually managed by DatabaseFixture, but other infra services might be registered here.
            // _services.AddScoped<IPatientRepository, PatientRepository>();

            // Register mocks for external dependencies
            // These will override any production implementations if registered with the same interface
            _services.AddSingleton<ILicensingApiClient, MockOdooLicensingApiClient>();
            _services.AddSingleton<ISmtpService, MockSmtpService>();
            
            // If IApplicationUpdateService is always mocked:
            // var mockUpdateService = new Mock<IApplicationUpdateService>();
            // _services.AddSingleton(mockUpdateService.Object); 
            // Or provide a concrete mock class similar to MockOdooLicensingApiClient

            // Register test helpers
            _services.AddSingleton<DicomTestDatasetManager>();
            // PerformanceMetricsHelper is static, no DI registration needed unless it becomes instance-based
            _services.AddSingleton<UiInteractionHelper>();

            // Register ViewModels if tests interact with them directly
            // Example:
            // _services.AddTransient<DicomImageViewerViewModel>();
            // _services.AddTransient<MainViewModel>(); // Or other relevant ViewModels

            _builtProvider = _services.BuildServiceProvider();
            ServiceProvider = _builtProvider;

            // Allow mocks to be further configured by tests if needed, by resolving their concrete mock types.
            // For example, a test could resolve MockOdooLicensingApiClient and call its setup methods.

            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            if (_builtProvider is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                _builtProvider?.Dispose();
            }
            await Task.CompletedTask;
        }

        public T GetService<T>() where T : notnull
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}