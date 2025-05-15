using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using TheSSS.DICOMViewer.Integration.Adapters;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Coordinators;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Policies;
using TheSSS.DICOMViewer.Integration.RateLimiting;
using TheSSS.DICOMViewer.Integration.Services;

// Assuming these dependencies are registered elsewhere as per the overall architecture:
// using TheSSS.DICOMViewer.Common.Logging; // For ILoggerAdapter<T>
// using TheSSS.DICOMViewer.Common.Security; // For ISecureStorageService
// using TheSSS.DICOMViewer.Infrastructure.Interfaces; // For IDicomLowLevelClient

namespace TheSSS.DICOMViewer.Integration.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection to register all gateway services, adapters,
    /// coordinators, and configurations for dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Service Integration Gateway components and configurations to the IServiceCollection.
        /// This method assumes that common services like ILoggerAdapter, ISecureStorageService, and IDicomLowLevelClient
        /// are registered by their respective modules/repositories before this is called.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the services to.</param>
        /// <param name="configuration">The application configuration, used to bind settings.</param>
        /// <returns>The IServiceCollection for chaining, allowing further service registrations.</returns>
        /// <exception cref="ArgumentNullException">Thrown if services or configuration is null.</exception>
        public static IServiceCollection AddServiceIntegrationGateway(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            // 1. Bind configuration sections to strongly-typed settings objects
            // These will be available via IOptions<T>
            services.Configure<ServiceGatewaySettings>(configuration.GetSection("ServiceGateway"));
            services.Configure<OdooApiSettings>(configuration.GetSection("ServiceGateway:OdooApi"));
            services.Configure<SmtpSettings>(configuration.GetSection("ServiceGateway:Smtp"));
            services.Configure<WindowsPrintSettings>(configuration.GetSection("ServiceGateway:WindowsPrint"));
            services.Configure<DicomGatewaySettings>(configuration.GetSection("ServiceGateway:Dicom"));
            services.Configure<ResilienceSettings>(configuration.GetSection("ServiceGateway:Resilience"));
            services.Configure<RateLimitSettings>(configuration.GetSection("ServiceGateway:RateLimiting"));
            services.Configure<CredentialManagerSettings>(configuration.GetSection("ServiceGateway:CredentialManager"));

            // 2. Register shared utilities/clients needed by adapters within this gateway
            // Register RestClient as a Singleton. Adapters that use it will get this instance.
            // Alternatively, use IHttpClientFactory for HttpClient based interactions.
            // Since RestSharp is specified, we register its client.
            services.AddSingleton<RestClient>(sp =>
            {
                // Basic RestClient. BaseUrl will be set in the OdooApiAdapter from its specific settings.
                // If RestClient needs more global configuration, it can be done here.
                return new RestClient();
            });

            // 3. Register core gateway services
            // These services are typically stateless or manage their state in a way suitable for Singleton.
            services.AddSingleton<IUnifiedErrorHandlingService, UnifiedErrorHandlingService>();
            services.AddSingleton<ICredentialManager, CredentialManager>();
            services.AddSingleton<IResiliencePolicyProvider, ResiliencePolicyProvider>();
            services.AddSingleton<IRateLimiter, ConfigurableRateLimiter>();
            // If a factory pattern was strictly required for RateLimiter:
            // services.AddSingleton<RateLimiterFactory>();
            // services.AddSingleton<IRateLimiter>(provider =>
            //     provider.GetRequiredService<RateLimiterFactory>().CreateRateLimiter());


            // 4. Register service adapters
            // Adapters are often stateless but can be Scoped or Transient if they manage
            // per-request/per-operation state or resources. Transient is a safe default.
            services.AddTransient<IOdooApiAdapter, OdooApiAdapter>();
            services.AddTransient<ISmtpServiceAdapter, SmtpServiceAdapter>();
            services.AddTransient<IWindowsPrintAdapter, WindowsPrintAdapter>();
            services.AddTransient<IDicomNetworkAdapter, DicomNetworkAdapter>();

            // 5. Register the main coordinator/facade
            // The coordinator orchestrates calls to adapters. Transient or Scoped is appropriate.
            services.AddTransient<IExternalServiceCoordinator, ExternalServiceCoordinator>();

            return services;
        }
    }
}