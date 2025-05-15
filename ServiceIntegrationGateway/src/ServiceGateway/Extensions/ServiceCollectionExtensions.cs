using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheSSS.DICOMViewer.Integration.Adapters;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Coordinators;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Policies;
using TheSSS.DICOMViewer.Integration.RateLimiting;
using TheSSS.DICOMViewer.Integration.Services;

// Assuming REPO-INFRA defines IDicomLowLevelClient in a namespace like TheSSS.DICOMViewer.Infrastructure.Dicom
// using TheSSS.DICOMViewer.Infrastructure.Dicom; 
// Assuming REPO-CROSS-CUTTING defines ILoggerAdapter, IConfigurationService etc.
// using TheSSS.DICOMViewer.CrossCutting.Logging;
// using TheSSS.DICOMViewer.CrossCutting.Configuration;
// using TheSSS.DICOMViewer.CrossCutting.Security;

namespace TheSSS.DICOMViewer.Integration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceIntegrationGateway(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Bind configuration settings
            // Main settings container
            services.Configure<ServiceGatewaySettings>(configuration.GetSection(nameof(ServiceGatewaySettings)));

            // Specific settings for adapters and services
            // These are assumed to be nested under ServiceGatewaySettings in the configuration file,
            // e.g., "ServiceGatewaySettings:OdooApiSettings"
            services.Configure<OdooApiSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(OdooApiSettings)}"));
            services.Configure<SmtpSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(SmtpSettings)}"));
            services.Configure<WindowsPrintSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(WindowsPrintSettings)}"));
            services.Configure<DicomGatewaySettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(DicomGatewaySettings)}"));
            services.Configure<ResilienceSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(ResilienceSettings)}"));
            services.Configure<RateLimitSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(RateLimitSettings)}"));
            services.Configure<CredentialManagerSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(CredentialManagerSettings)}"));

            // Register Adapters
            services.AddScoped<IOdooApiAdapter, OdooApiAdapter>();
            services.AddScoped<ISmtpServiceAdapter, SmtpServiceAdapter>();
            services.AddScoped<IWindowsPrintAdapter, WindowsPrintAdapter>();
            services.AddScoped<IDicomNetworkAdapter, DicomNetworkAdapter>();

            // Register Services
            services.AddScoped<ICredentialManager, CredentialManager>();
            services.AddScoped<IUnifiedErrorHandlingService, UnifiedErrorHandlingService>();

            // Register Resilience Policy Provider
            // Policies are typically configured once and reused, so Singleton is appropriate.
            services.AddSingleton<IResiliencePolicyProvider, ResiliencePolicyProvider>();

            // Register Rate Limiting
            // ConfigurableRateLimiter will manage multiple internal limiters based on configuration.
            // It's stateful (maintains limiter states) and shared, so Singleton.
            services.AddSingleton<IRateLimiter, ConfigurableRateLimiter>();
            // If RateLimiterFactory is a separate entity for creating RateLimiter instances and is used by ConfigurableRateLimiter,
            // it might be registered here as well, e.g., services.AddSingleton<RateLimiterFactory>();
            // However, based on the description, ConfigurableRateLimiter seems to be the primary IRateLimiter implementation.

            // Register Coordinator
            services.AddScoped<IExternalServiceCoordinator, ExternalServiceCoordinator>();

            // Note: Dependencies from external repositories like IDicomLowLevelClient (REPO-INFRA)
            // and ILoggerAdapter (REPO-CROSS-CUTTING) are assumed to be registered 
            // by the main application's composition root.

            return services;
        }
    }
}