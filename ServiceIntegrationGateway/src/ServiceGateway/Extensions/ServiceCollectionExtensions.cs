using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using RestSharp;
using TheSSS.DICOMViewer.Integration.Adapters;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Coordinators;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Policies;
using TheSSS.DICOMViewer.Integration.RateLimiting;
using TheSSS.DICOMViewer.Integration.Services;
// Assuming ILoggerAdapter and ISecureDataStorage are in TheSSS.DICOMViewer.Common.Interfaces
// Assuming IDicomLowLevelClient is in TheSSS.DICOMViewer.Infrastructure.Interfaces
// These would be registered by their respective projects/application host.

namespace TheSSS.DICOMViewer.Integration.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register ServiceIntegrationGateway components.
/// Simplifies the registration of all necessary services and components from the ServiceIntegrationGateway
/// into the application's main IServiceCollection, including configuration options binding.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures all services, adapters, and configurations for the ServiceIntegrationGateway.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The IConfiguration containing gateway settings.</param>
    /// <returns>The modified IServiceCollection.</returns>
    public static IServiceCollection AddServiceIntegrationGateway(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Bind configuration sections using IOptions pattern
        // This makes the settings available via IOptions<T> injection.
        services.Configure<ServiceGatewaySettings>(configuration.GetSection(nameof(ServiceGatewaySettings)));
        services.Configure<OdooApiSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(ServiceGatewaySettings.OdooApi)}"));
        services.Configure<SmtpSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(ServiceGatewaySettings.Smtp)}"));
        services.Configure<WindowsPrintSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(ServiceGatewaySettings.WindowsPrint)}"));
        services.Configure<DicomGatewaySettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(ServiceGatewaySettings.DicomGateway)}"));
        services.Configure<ResilienceSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(ServiceGatewaySettings.Resilience)}"));
        services.Configure<RateLimitSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(ServiceGatewaySettings.RateLimiting)}"));
        services.Configure<CredentialManagerSettings>(configuration.GetSection($"{nameof(ServiceGatewaySettings)}:{nameof(ServiceGatewaySettings.CredentialManager)}"));

        // 2. Register Policies and Rate Limiting components
        // ResiliencePolicyProvider manages Polly policies and should be a singleton.
        services.AddSingleton<IResiliencePolicyProvider, ResiliencePolicyProvider>();

        // ConfigurableRateLimiter manages System.Threading.RateLimiting instances and should be a singleton.
        services.AddSingleton<IRateLimiter, ConfigurableRateLimiter>();
        // The RateLimiterFactory itself is not directly registered as an interface implementation here,
        // as ConfigurableRateLimiter is the primary IRateLimiter.
        // If RateLimiterFactory were to be used to create IRateLimiter, it would be:
        // services.AddSingleton<RateLimiterFactory>();
        // services.AddSingleton<IRateLimiter>(sp => sp.GetRequiredService<RateLimiterFactory>().Create());
        // But direct registration of ConfigurableRateLimiter is simpler.

        // 3. Register core services of the gateway
        // CredentialManager and UnifiedErrorHandlingService are typically singletons.
        services.AddSingleton<ICredentialManager, CredentialManager>();
        services.AddSingleton<IUnifiedErrorHandlingService, UnifiedErrorHandlingService>();

        // Register RestClient as a singleton as recommended for modern RestSharp versions
        // OdooApiAdapter will consume this.
        services.AddSingleton<RestClient>();

        // 4. Register Adapters
        // Adapters are generally stateless and can be singletons.
        // If they had scoped dependencies or held per-request state, they might be Scoped or Transient.
        services.AddSingleton<IOdooApiAdapter, OdooApiAdapter>();
        services.AddSingleton<ISmtpServiceAdapter, SmtpServiceAdapter>();
        services.AddSingleton<IWindowsPrintAdapter, WindowsPrintAdapter>();
        services.AddSingleton<IDicomNetworkAdapter, DicomNetworkAdapter>();

        // 5. Register the main Coordinator (Facade)
        // The ExternalServiceCoordinator is the primary entry point to the gateway and can be a singleton.
        services.AddSingleton<IExternalServiceCoordinator, ExternalServiceCoordinator>();

        // Note: Dependencies from other repositories/projects such as:
        // - TheSSS.DICOMViewer.Common.Interfaces.ILoggerAdapter
        // - TheSSS.DICOMViewer.Common.Interfaces.ISecureDataStorage (if used by CredentialManager)
        // - TheSSS.DICOMViewer.Infrastructure.Interfaces.IDicomLowLevelClient
        // are assumed to be registered in the main application's DI container (e.g., in Program.cs or a startup class).
        // This extension method focuses solely on registering components *within* the ServiceIntegrationGateway.

        return services;
    }
}