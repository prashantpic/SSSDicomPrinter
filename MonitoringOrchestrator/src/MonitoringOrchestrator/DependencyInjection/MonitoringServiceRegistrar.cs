using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheSSS.DICOMViewer.Monitoring.Alerting.Channels;
using TheSSS.DICOMViewer.Monitoring.Alerting.Strategies;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.HealthDataSources;
using TheSSS.DICOMViewer.Monitoring.Integrations;
using TheSSS.DICOMViewer.Monitoring.Integrations.HealthChecks;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;
using TheSSS.DICOMViewer.Monitoring.Validators;

namespace TheSSS.DICOMViewer.Monitoring.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/>
    /// to register monitoring services and their dependencies.
    /// </summary>
    public static class MonitoringServiceRegistrar
    {
        /// <summary>
        /// Adds the monitoring orchestrator services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance for configuration binding.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddMonitoringServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register FluentValidation validators from the assembly containing MonitoringOptionsValidator
            // Validators are typically registered with a scoped lifetime, which is the default.
            services.AddValidatorsFromAssemblyContaining<MonitoringOptionsValidator>();

            // Bind, validate, and register MonitoringOptions
            services.AddOptions<MonitoringOptions>()
                .Bind(configuration.GetSection("Monitoring")) // As per example JSON structure
                .ValidateFluentValidation()
                .ValidateOnStart();

            // Bind, validate, and register AlertingOptions
            services.AddOptions<AlertingOptions>()
                .Bind(configuration.GetSection("Alerting")) // As per example JSON structure
                .ValidateFluentValidation()
                .ValidateOnStart();

            // Register UseCaseHandlers
            services.AddScoped<HealthAggregationService>();
            services.AddScoped<AlertEvaluationService>();
            services.AddScoped<AlertDispatchService>();

            // Register Background Worker
            services.AddHostedService<SystemHealthMonitorWorker>();

            // Register IHealthDataSource implementations
            services.AddScoped<IHealthDataSource, StorageHealthDataSource>();
            services.AddScoped<IHealthDataSource, DatabaseConnectivityDataSource>();
            services.AddScoped<IHealthDataSource, PacsStatusDataSource>();
            services.AddScoped<IHealthDataSource, LicenseStatusDataSource>();
            services.AddScoped<IHealthDataSource, SystemErrorDataSource>();
            services.AddScoped<IHealthDataSource, AutomatedTaskStatusDataSource>();

            // Register IAlertingChannel implementations
            services.AddScoped<IAlertingChannel, EmailAlertingChannel>();
            services.AddScoped<IAlertingChannel, UiNotificationChannel>();
            services.AddScoped<IAlertingChannel, AuditLogAlertingChannel>();

            // Register IAlertThrottlingStrategy (Singleton as it likely holds state)
            services.AddSingleton<IAlertThrottlingStrategy, DefaultAlertThrottlingStrategy>();

            // Register IAlertDeduplicationStrategy (Singleton as it likely holds state)
            services.AddSingleton<IAlertDeduplicationStrategy, DefaultAlertDeduplicationStrategy>();
            
            // Register IAlertRuleConfigProvider (Assuming a simple implementation that reads from IOptions<AlertingOptions>)
            // If AlertEvaluationService directly uses IOptions<AlertingOptions>.Value.Rules, this might be a pass-through or not strictly needed.
            // For adherence to the specified design, we register it.
            services.AddScoped<IAlertRuleConfigProvider, OptionsAlertRuleConfigProvider>();

            // Register Prometheus integration
            services.AddSingleton<PrometheusMetricsCollector>();

            // Register ASP.NET Core Health Check
            services.AddHealthChecks()
                .AddCheck<DicomSystemHealthCheck>("dicom_system_health", HealthStatus.Unhealthy, tags: new[] { "monitoring", "system" });

            return services;
        }
    }

    // Placeholder for OptionsAlertRuleConfigProvider if it's a simple wrapper.
    // This would typically be in its own file under a relevant namespace like 'Providers' or 'Services'.
    // For the purpose of this DI registration file, we assume its existence.
    // Actual implementation would inject IOptions<AlertingOptions> and return options.Value.Rules.
    /// <summary>
    /// Provides alert rules by reading them from configured <see cref="AlertingOptions"/>.
    /// </summary>
    internal class OptionsAlertRuleConfigProvider : IAlertRuleConfigProvider
    {
        private readonly Microsoft.Extensions.Options.IOptions<AlertingOptions> _alertingOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsAlertRuleConfigProvider"/> class.
        /// </summary>
        /// <param name="alertingOptions">The alerting options.</param>
        public OptionsAlertRuleConfigProvider(Microsoft.Extensions.Options.IOptions<AlertingOptions> alertingOptions)
        {
            _alertingOptions = alertingOptions ?? throw new ArgumentNullException(nameof(alertingOptions));
        }

        /// <inheritdoc/>
        public Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken)
        {
            // Ensure options and Rules list are not null to prevent NullReferenceException
            var rules = _alertingOptions.Value?.Rules ?? Enumerable.Empty<AlertRule>();
            return Task.FromResult(rules.AsEnumerable());
        }
    }
}