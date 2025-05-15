using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Validators;
using TheSSS.DICOMViewer.Monitoring.HealthDataSources;
using TheSSS.DICOMViewer.Monitoring.Alerting.Channels;
using TheSSS.DICOMViewer.Monitoring.Alerting.Strategies;
using TheSSS.DICOMViewer.Monitoring.Integrations;
using TheSSS.DICOMViewer.Monitoring.Integrations.HealthChecks;
using Microsoft.Extensions.Options;
using FluentValidation;
using System.Linq;
using TheSSS.DICOMViewer.Monitoring.Mappers;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using System;
using System.Collections.Generic; // Required for IEnumerable

namespace TheSSS.DICOMViewer.Monitoring.DependencyInjection
{
    public static class MonitoringServiceRegistrar
    {
        /// <summary>
        /// Adds all MonitoringOrchestrator services, data sources, channels, strategies,
        /// and configuration bindings to the IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to register services with.</param>
        /// <param name="configuration">The IConfiguration containing monitoring and alerting settings.</param>
        /// <returns>The modified IServiceCollection.</returns>
        public static IServiceCollection AddMonitoringServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // --- 1. Configuration Binding and Validation ---
            services.AddOptions<MonitoringOptions>()
                .Bind(configuration.GetSection(MonitoringOptions.SectionName))
                .ValidateDataAnnotations() // If using Data Annotations on POCOs
                .ValidateFluentValidation() // Use custom FluentValidation
                .ValidateOnStart(); // Validate configuration on application startup

            services.AddOptions<AlertingOptions>()
                .Bind(configuration.GetSection(AlertingOptions.SectionName))
                .ValidateDataAnnotations() // If using Data Annotations on POCOs
                .ValidateFluentValidation() // Use custom FluentValidation
                .ValidateOnStart(); // Validate configuration on application startup

            // Register FluentValidators for options POCOs
            services.AddScoped<IValidator<MonitoringOptions>, MonitoringOptionsValidator>();
            services.AddScoped<IValidator<AlertingOptions>, AlertingOptionsValidator>();
            // Note: Validators for nested types like AlertRule are typically handled by SetValidator in the parent validator.
            // If they need to be resolved independently, register them here.
            // services.AddScoped<IValidator<AlertRule>, AlertRuleValidator>();
            // services.AddScoped<IValidator<AlertChannelSetting>, AlertChannelSettingValidator>();
            // services.AddScoped<IValidator<ThrottlingOptions>, ThrottlingOptionsValidator>();
            // services.AddScoped<IValidator<DeduplicationOptions>, DeduplicationOptionsValidator>();


            // --- 2. Core Use Case Handlers ---
            // These are typically scoped as they might be used within a request or a worker cycle
            services.AddScoped<HealthAggregationService>();
            services.AddScoped<AlertEvaluationService>();
            services.AddScoped<AlertDispatchService>();
            
            // HealthReportMapper is static, so no DI registration for the class itself.
            // If it were an instance class, it would be registered, likely as Singleton if stateless.


            // --- 3. Health Data Sources (Implementations of IHealthDataSource) ---
            // Register each data source as a scoped service and then as IHealthDataSource
            // This allows them to have their own scoped dependencies if needed.
            // Adapters they depend on (e.g., IStorageInfoAdapter) must be registered elsewhere.
            services.AddScoped<StorageHealthDataSource>();
            services.AddScoped<IHealthDataSource>(provider => provider.GetRequiredService<StorageHealthDataSource>());

            services.AddScoped<DatabaseConnectivityDataSource>();
            services.AddScoped<IHealthDataSource>(provider => provider.GetRequiredService<DatabaseConnectivityDataSource>());

            services.AddScoped<PacsStatusDataSource>();
            services.AddScoped<IHealthDataSource>(provider => provider.GetRequiredService<PacsStatusDataSource>());

            services.AddScoped<LicenseStatusDataSource>();
            services.AddScoped<IHealthDataSource>(provider => provider.GetRequiredService<LicenseStatusDataSource>());

            services.AddScoped<SystemErrorDataSource>();
            services.AddScoped<IHealthDataSource>(provider => provider.GetRequiredService<SystemErrorDataSource>());

            services.AddScoped<AutomatedTaskStatusDataSource>();
            services.AddScoped<IHealthDataSource>(provider => provider.GetRequiredService<AutomatedTaskStatusDataSource>());

            // The HealthAggregationService will receive IEnumerable<IHealthDataSource> via DI.


            // --- 4. Alerting Channels (Implementations of IAlertingChannel) ---
            // Register each channel similarly. Adapters they depend on must be registered elsewhere.
            services.AddScoped<EmailAlertingChannel>();
            services.AddScoped<IAlertingChannel>(provider => provider.GetRequiredService<EmailAlertingChannel>());

            services.AddScoped<UiNotificationChannel>();
            services.AddScoped<IAlertingChannel>(provider => provider.GetRequiredService<UiNotificationChannel>());

            services.AddScoped<AuditLogAlertingChannel>();
            services.AddScoped<IAlertingChannel>(provider => provider.GetRequiredService<AuditLogAlertingChannel>());

            // The AlertDispatchService will receive IEnumerable<IAlertingChannel> via DI.


            // --- 5. Alerting Strategies (Implementations of IAlertThrottlingStrategy, IAlertDeduplicationStrategy) ---
            // Strategies often hold state (in-memory caches for timestamps/counts) and should be Singletons.
            services.AddSingleton<IAlertThrottlingStrategy, DefaultAlertThrottlingStrategy>();
            services.AddSingleton<IAlertDeduplicationStrategy, DefaultAlertDeduplicationStrategy>();

            // --- 6. Alert Rule Config Provider (if not directly using IOptions<AlertingOptions>) ---
            // Example: A simple provider that gets rules from IOptions.
            // If rules were from DB or another source, a different implementation would be registered.
            services.AddScoped<IAlertRuleConfigProvider, OptionsAlertRuleConfigProvider>();


            // --- 7. Background Worker ---
            // Register the main monitoring worker as a BackgroundService (Hosted Service).
            services.AddHostedService<SystemHealthMonitorWorker>();

            // --- 8. Integrations ---
            // Register Prometheus collector. It manages global Prometheus metrics, so Singleton.
            services.AddSingleton<PrometheusMetricsCollector>();

            // Register ASP.NET Core Health Check implementation.
            // Health checks are typically registered as Transient.
            // The actual addition to the health check pipeline (`.AddHealthChecks().AddCheck<DicomSystemHealthCheck>(...)`)
            // is done in the hosting application's startup.
            services.AddTransient<DicomSystemHealthCheck>();


            // --- 9. Adapters Interfaces ---
            // Implementations of adapter interfaces (IAuditLoggingAdapter, IEmailServiceAdapter, etc.)
            // are NOT registered here. They are expected to be registered by the consuming application
            // or dedicated infrastructure/adapter projects. This module defines the contracts (interfaces)
            // it needs, and the host application fulfills these contracts via DI.

            return services;
        }

        // Helper method for FluentValidation options validation
        public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
        {
            optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
                provider => new FluentValidationValidateOptions<TOptions>(
                    optionsBuilder.Name, // Pass the options name for named options support
                    provider.GetRequiredService<IValidator<TOptions>>()));
            return optionsBuilder;
        }
    }

    // Internal helper class for FluentValidation integration with IOptions
    // This allows using FluentValidation for IOptions validation pattern.
    internal class FluentValidationValidateOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
    {
        private readonly string? _name;
        private readonly IValidator<TOptions> _validator;

        public FluentValidationValidateOptions(string? name, IValidator<TOptions> validator)
        {
            _name = name; // The name of the options instance being validated
            _validator = validator;
        }

        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            // Null name is used for all unnamed options instances.
            // Ensure this validation applies only to the options instance intended.
            if (_name != null && _name != name)
            {
                // Not the options instance that this validator applies to.
                return ValidateOptionsResult.Skip;
            }

            ArgumentNullException.ThrowIfNull(options);

            var validationResult = _validator.Validate(options);
            if (validationResult.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            var errors = validationResult.Errors.Select(
                failure => $"Configuration validation error for '{typeof(TOptions).Name}{(_name == null ? "" : $":{_name}")}': {failure.PropertyName} - {failure.ErrorMessage}").ToList();
            
            // For ValidateOnStart behavior, this will throw ConfigurationValidationException
            // The exception should be thrown by the validation system when ValidateOnStart is used.
            // Here, we just return the failure result.
            // If a custom exception is desired directly from here, it could be thrown,
            // but standard IOptions validation throws OptionsValidationException.
            // The original spec mentioned ConfigurationValidationException.
            // To achieve that, we'd need to ensure this is the primary validation mechanism that throws.
            // For ValidateOnStart, the host typically wraps the IValidateOptions.Validate call.
            // If this call results in Fail, OptionsValidationException containing the failures is thrown.
            // To throw a specific *custom* exception like ConfigurationValidationException,
            // one might need to create a custom IHostedService that performs validation and throws,
            // or ensure the IOptionsMonitor.Get call is wrapped.
            // For now, let's stick to standard IValidateOptions behavior and rely on
            // the hosting system to throw OptionsValidationException upon failure with ValidateOnStart.
            // If a ConfigurationValidationException is strictly required, the services.AddOptions line might need
            // a custom .PostConfigure or a custom validation action that throws it.
            // The spec had a ConfigurationValidationException in the Exceptions folder.
            // Let's assume this validator, when `ValidateOnStart` is used,
            // should result in a `ConfigurationValidationException` rather than `OptionsValidationException`.
            // One way is to throw it from here if the intent is to always crash with this specific type.
            if (name == Microsoft.Extensions.Options.Options.DefaultName) // Check if it's the default instance often used by ValidateOnStart
            {
                 // This part is tricky. IValidateOptions is expected to return ValidateOptionsResult.
                 // Throwing directly from here might bypass some IOptions mechanisms.
                 // The host typically throws OptionsValidationException.
                 // To meet the ConfigurationValidationException requirement, one might register a startup task.
                 // However, if we must throw from here:
                 // throw new ConfigurationValidationException($"Configuration validation failed for {typeof(TOptions).Name}.", errors);
                 // This might be too aggressive for IValidateOptions. Let's return Fail, and rely on
                 // the host's ValidateOnStart mechanism to throw (which would be OptionsValidationException).
                 // The previous example solution did throw ConfigurationValidationException. Let's align.
                 // The most direct way to use ConfigurationValidationException with ValidateOnStart
                 // would be to customize the host's startup validation or use a validation action
                 // on the options builder. Let's assume for now the provided `FluentValidationValidateOptions`
                 // should throw directly to match the example's custom exception.
                 throw new ConfigurationValidationException($"Configuration validation failed for {typeof(TOptions).Name}{(_name == null ? "" : $":{_name}")}.", errors);
            }


            return ValidateOptionsResult.Fail(errors);
        }
    }

    /// <summary>
    /// Simple implementation of IAlertRuleConfigProvider that sources rules from IOptions.
    /// </summary>
    internal class OptionsAlertRuleConfigProvider : IAlertRuleConfigProvider
    {
        private readonly AlertingOptions _alertingOptions;

        public OptionsAlertRuleConfigProvider(IOptions<AlertingOptions> alertingOptions)
        {
            _alertingOptions = alertingOptions.Value;
        }

        public Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_alertingOptions.Rules?.AsEnumerable() ?? Enumerable.Empty<AlertRule>());
        }
    }
}