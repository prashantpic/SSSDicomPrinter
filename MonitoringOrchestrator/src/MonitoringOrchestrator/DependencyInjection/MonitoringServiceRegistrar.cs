using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Required for BackgroundService
using TheSSS.DICOMViewer.Monitoring.Alerting.Channels;
using TheSSS.DICOMViewer.Monitoring.Alerting.Strategies;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.HealthDataSources;
using TheSSS.DICOMViewer.Monitoring.Integrations;
using TheSSS.DICOMViewer.Monitoring.Integrations.HealthChecks;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Mappers;
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;
using TheSSS.DICOMViewer.Monitoring.Validators;
using Microsoft.Extensions.Diagnostics.HealthChecks; // Required for IHealthCheck
using Microsoft.Extensions.Options; // Required for AddOptions
using TheSSS.DICOMViewer.Monitoring.Exceptions; // For ConfigurationValidationException
using System.Linq; // For LINQ methods like .Select, .Any
using System.Collections.Generic; // For List<T>
using System.ComponentModel.DataAnnotations; // For ValidationContext, ValidationResult
using System.Threading.Tasks; // For Task
using System.Threading; // For CancellationToken

namespace TheSSS.DICOMViewer.Monitoring.DependencyInjection;

public static class MonitoringServiceRegistrar
{
    public static IServiceCollection AddMonitoringServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configuration Options Binding and Validation
        services.AddOptions<MonitoringOptions>()
                .Bind(configuration.GetSection("Monitoring"))
                .ValidateDataAnnotations() // Validates top-level Data Annotations
                .ValidateWithDataAnnotationExtensions() // Validates nested objects using Data Annotations
                .ValidateOnStart(); // Ensures validation runs at startup

        services.AddOptions<AlertingOptions>()
                .Bind(configuration.GetSection("Alerting"))
                .ValidateDataAnnotations()
                .ValidateWithDataAnnotationExtensions()
                .ValidateOnStart();

        // Register Fluent Validators for IOptions<T>
        services.AddSingleton<IValidateOptions<MonitoringOptions>, MonitoringOptionsValidatorAsValidateOptions>();
        services.AddSingleton<IValidateOptions<AlertingOptions>, AlertingOptionsValidatorAsValidateOptions>();

        // Register individual Fluent Validators for direct use if needed (e.g., nested validation)
        services.AddTransient<IValidator<MonitoringOptions>, MonitoringOptionsValidator>();
        services.AddTransient<IValidator<AlertingOptions>, AlertingOptionsValidator>();
        services.AddTransient<IValidator<AlertRule>, AlertRuleValidator>();
        services.AddTransient<IValidator<AlertChannelSetting>, AlertChannelSettingValidator>();
        services.AddTransient<IValidator<ThrottlingOptions>, ThrottlingOptionsValidator>();
        services.AddTransient<IValidator<DeduplicationOptions>, DeduplicationOptionsValidator>();


        // 2. Use Case Handlers / Services
        services.AddScoped<HealthAggregationService>();
        services.AddScoped<AlertEvaluationService>();
        services.AddScoped<AlertDispatchService>();
        services.AddScoped<HealthReportMapper>(); // Mappers as Scoped services

        // 3. Background Worker
        services.AddHostedService<SystemHealthMonitorWorker>();

        // 4. Health Data Sources (Register as IHealthDataSource)
        services.AddScoped<IHealthDataSource, StorageHealthDataSource>();
        services.AddScoped<IHealthDataSource, DatabaseConnectivityDataSource>();
        services.AddScoped<IHealthDataSource, PacsStatusDataSource>();
        services.AddScoped<IHealthDataSource, LicenseStatusDataSource>();
        services.AddScoped<IHealthDataSource, SystemErrorDataSource>();
        services.AddScoped<IHealthDataSource, AutomatedTaskStatusDataSource>();

        // 5. Alerting Channels (Register as IAlertingChannel)
        services.AddScoped<IAlertingChannel, EmailAlertingChannel>();
        services.AddScoped<IAlertingChannel, UiNotificationChannel>();
        services.AddScoped<IAlertingChannel, AuditLogAlertingChannel>();

        // 6. Alerting Strategies (Register as their interfaces)
        services.AddSingleton<IAlertThrottlingStrategy, DefaultAlertThrottlingStrategy>();
        services.AddSingleton<IAlertDeduplicationStrategy, DefaultAlertDeduplicationStrategy>();

        // 7. Adapter Interfaces
        // These are expected to be registered by other modules or the main application host.
        // Examples:
        // services.AddScoped<IAuditLoggingAdapter, ConcreteAuditLoggingAdapter>();
        // services.AddScoped<IEmailServiceAdapter, ConcreteEmailServiceAdapter>();
        // ...and so on for all adapter interfaces defined in TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters

        // Assuming ILoggerAdapter is also provided externally (e.g., from a Cross-Cutting module)
        // services.AddSingleton(typeof(ILoggerAdapter<>), typeof(ConcreteLoggerAdapter<>));

        // Implementation for IAlertRuleConfigProvider (wraps IOptions)
        services.AddScoped<IAlertRuleConfigProvider, OptionsAlertRuleConfigProvider>();

        // 8. Integrations
        services.AddSingleton<PrometheusMetricsCollector>(); // Singleton as it manages global Prometheus metrics
        
        // ASP.NET Core Health Check registration
        services.AddHealthChecks()
                .AddCheck<DicomSystemHealthCheck>("DicomSystemHealthCheck",
                                                  failureStatus: HealthStatus.Degraded, // Or Unhealthy based on policy
                                                  tags: new[] { "live", "ready" });

        return services;
    }
}

// Helper class to integrate FluentValidation with IOptions validation for MonitoringOptions
public class MonitoringOptionsValidatorAsValidateOptions : IValidateOptions<MonitoringOptions>
{
    private readonly IValidator<MonitoringOptions> _validator;
    public MonitoringOptionsValidatorAsValidateOptions(IValidator<MonitoringOptions> validator) => _validator = validator;

    public ValidateOptionsResult Validate(string? name, MonitoringOptions options)
    {
        if (options == null) return ValidateOptionsResult.Fail("MonitoringOptions cannot be null.");
        
        var validationResult = _validator.Validate(options);
        if (validationResult.IsValid) return ValidateOptionsResult.Success;

        var errors = validationResult.Errors.Select(e => $"FluentValidation failed for {e.PropertyName}: {e.ErrorMessage}");
        return ValidateOptionsResult.Fail(errors);
    }
}

// Helper class to integrate FluentValidation with IOptions validation for AlertingOptions
public class AlertingOptionsValidatorAsValidateOptions : IValidateOptions<AlertingOptions>
{
    private readonly IValidator<AlertingOptions> _validator;
    public AlertingOptionsValidatorAsValidateOptions(IValidator<AlertingOptions> validator) => _validator = validator;

    public ValidateOptionsResult Validate(string? name, AlertingOptions options)
    {
        if (options == null) return ValidateOptionsResult.Fail("AlertingOptions cannot be null.");

        var validationResult = _validator.Validate(options);
        if (validationResult.IsValid) return ValidateOptionsResult.Success;

        var errors = validationResult.Errors.Select(e => $"FluentValidation failed for {e.PropertyName}: {e.ErrorMessage}");
        return ValidateOptionsResult.Fail(errors);
    }
}

// Simple implementation of IAlertRuleConfigProvider that wraps IOptions<AlertingOptions>
public class OptionsAlertRuleConfigProvider : IAlertRuleConfigProvider
{
    private readonly AlertingOptions _alertingOptions;
    public OptionsAlertRuleConfigProvider(IOptions<AlertingOptions> alertingOptions)
    {
        _alertingOptions = alertingOptions.Value; // IOptions.Value gives the configured instance
    }

    public Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<AlertRule>>(_alertingOptions.Rules ?? new List<AlertRule>());
    }
}

// Extension method for IOptionsBuilder to validate nested objects using Data Annotations
public static class OptionsBuilderDataAnnotationExtensions
{
    public static OptionsBuilder<TOptions> ValidateWithDataAnnotationExtensions<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.PostConfigure(options =>
        {
            var validationContext = new ValidationContext(options, serviceProvider: null, items: null);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            // Validator.TryValidateObject recursively validates properties with [ValidateObject] (if defined)
            // and other DataAnnotation attributes on the object and its properties.
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(options, validationContext, validationResults, validateAllProperties: true);

            if (!isValid && validationResults.Any())
            {
                // Convert DataAnnotation ValidationResult to FluentValidation's ValidationFailure for consistency
                // if throwing ConfigurationValidationException that expects IEnumerable<ValidationFailure>.
                var fluentFailures = validationResults
                    .Select(r => new ValidationFailure(r.MemberNames.FirstOrDefault() ?? "UnknownProperty", r.ErrorMessage ?? "Validation error"))
                    .ToList();

                throw new ConfigurationValidationException(
                    fluentFailures,
                    "Configuration validation failed using Data Annotations on nested objects.");
            }
        });
        return optionsBuilder;
    }
}