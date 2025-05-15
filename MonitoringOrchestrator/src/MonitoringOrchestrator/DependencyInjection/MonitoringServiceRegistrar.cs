using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Validators;
using TheSSS.DICOMViewer.Monitoring.HealthDataSources;
using TheSSS.DICOMViewer.Monitoring.Alerting.Channels;
using TheSSS.DICOMViewer.Monitoring.Alerting.Strategies;
using TheSSS.DICOMViewer.Monitoring.Integrations;
using Microsoft.Extensions.Options;
using FluentValidation;

namespace TheSSS.DICOMViewer.Monitoring.DependencyInjection;

public static class MonitoringServiceRegistrar
{
    public static IServiceCollection AddMonitoringServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MonitoringOptions>()
            .Bind(configuration.GetSection(MonitoringOptions.SectionName))
            .ValidateFluentValidation()
            .ValidateOnStart();

        services.AddOptions<AlertingOptions>()
            .Bind(configuration.GetSection(AlertingOptions.SectionName))
            .ValidateFluentValidation()
            .ValidateOnStart();

        services.AddScoped<IValidator<MonitoringOptions>, MonitoringOptionsValidator>();
        services.AddScoped<IValidator<AlertingOptions>, AlertingOptionsValidator>();

        services.AddScoped<HealthAggregationService>();
        services.AddScoped<AlertEvaluationService>();
        services.AddScoped<AlertDispatchService>();
        services.AddSingleton<HealthReportMapper>();

        RegisterHealthDataSources(services);
        RegisterAlertingChannels(services);
        RegisterStrategies(services);
        RegisterIntegrations(services);

        services.AddHostedService<SystemHealthMonitorWorker>();

        return services;
    }

    private static void RegisterHealthDataSources(IServiceCollection services)
    {
        services.AddScoped<StorageHealthDataSource>();
        services.AddScoped<IHealthDataSource>(p => p.GetRequiredService<StorageHealthDataSource>());

        services.AddScoped<DatabaseConnectivityDataSource>();
        services.AddScoped<IHealthDataSource>(p => p.GetRequiredService<DatabaseConnectivityDataSource>());

        services.AddScoped<PacsStatusDataSource>();
        services.AddScoped<IHealthDataSource>(p => p.GetRequiredService<PacsStatusDataSource>());

        services.AddScoped<LicenseStatusDataSource>();
        services.AddScoped<IHealthDataSource>(p => p.GetRequiredService<LicenseStatusDataSource>());

        services.AddScoped<SystemErrorDataSource>();
        services.AddScoped<IHealthDataSource>(p => p.GetRequiredService<SystemErrorDataSource>());

        services.AddScoped<AutomatedTaskStatusDataSource>();
        services.AddScoped<IHealthDataSource>(p => p.GetRequiredService<AutomatedTaskStatusDataSource>());
    }

    private static void RegisterAlertingChannels(IServiceCollection services)
    {
        services.AddScoped<EmailAlertingChannel>();
        services.AddScoped<IAlertingChannel>(p => p.GetRequiredService<EmailAlertingChannel>());

        services.AddScoped<UiNotificationChannel>();
        services.AddScoped<IAlertingChannel>(p => p.GetRequiredService<UiNotificationChannel>());

        services.AddScoped<AuditLogAlertingChannel>();
        services.AddScoped<IAlertingChannel>(p => p.GetRequiredService<AuditLogAlertingChannel>());
    }

    private static void RegisterStrategies(IServiceCollection services)
    {
        services.AddSingleton<IAlertThrottlingStrategy, DefaultAlertThrottlingStrategy>();
        services.AddSingleton<IAlertDeduplicationStrategy, DefaultAlertDeduplicationStrategy>();
    }

    private static void RegisterIntegrations(IServiceCollection services)
    {
        services.AddSingleton<PrometheusMetricsCollector>();
    }

    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            provider => new FluentValidationValidateOptions<TOptions>(
                optionsBuilder.Name,
                provider.GetRequiredService<IValidator<TOptions>>()));
        return optionsBuilder;
    }

    private class FluentValidationValidateOptions<TOptions>(string? name, IValidator<TOptions> validator) : IValidateOptions<TOptions> where TOptions : class
    {
        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            if (this.name != null && name != null && this.name != name)
                return ValidateOptionsResult.Skip;

            var result = validator.Validate(options);
            if (result.IsValid) return ValidateOptionsResult.Success;

            var errors = result.Errors.Select(f => $"{f.PropertyName}: {f.ErrorMessage}");
            return ValidateOptionsResult.Fail(errors);
        }
    }
}