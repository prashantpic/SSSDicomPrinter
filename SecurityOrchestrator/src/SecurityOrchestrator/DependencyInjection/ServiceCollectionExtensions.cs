using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheSSS.DICOMViewer.Security.Configuration;
using TheSSS.DICOMViewer.Security.Engines;
using TheSSS.DICOMViewer.Security.Services;
using TheSSS.DICOMViewer.Security.Validators;

namespace TheSSS.DICOMViewer.Security.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to simplify the registration
/// of all services, engines, validators, and configurations defined within the SecurityOrchestrator project.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services, engines, validators, and configurations from the SecurityOrchestrator project
    /// to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> to bind settings from.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <remarks>
    /// This method assumes that implementations for the interfaces defined in
    /// `TheSSS.DICOMViewer.Security.Interfaces` (e.g., `IAuditLogService`, `ILicenseApiClient`,
    /// `IIdentityProviderService`, `IRolePermissionProvider`, `ISensitiveDataProtector`,
    /// `IAlertingService`, `IPhiMaskingPolicyProvider`) are registered by other projects,
    /// typically the Infrastructure or Cross-Cutting layers, before this method is called.
    /// </remarks>
    public static IServiceCollection AddSecurityOrchestration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure settings
        services.Configure<SecurityOrchestratorSettings>(configuration.GetSection("SecurityOrchestrator"));

        // Register orchestration services
        services.AddScoped<ILicenseOrchestrationService, LicenseOrchestrationService>();
        services.AddScoped<IAuthenticationOrchestrationService, AuthenticationOrchestrationService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICryptographyOrchestrationService, CryptographyOrchestrationService>();
        services.AddScoped<IPhiMaskingCoordinatorService, PhiMaskingCoordinatorService>();

        // Register engines
        services.AddScoped<SecurityPolicyEngine>();

        // Register validators from this assembly
        services.AddValidatorsFromAssemblyContaining<AuthenticationRequestValidator>(ServiceLifetime.Scoped);

        return services;
    }
}