namespace TheSSS.DICOMViewer.Common.Logging;

public static class SerilogConfigurationExtensions
{
    public static IServiceCollection AddConfiguredSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.With<PhiMaskingEnricher>();

        Log.Logger = loggerConfiguration.CreateLogger();
        
        services.AddSingleton(Log.Logger);
        services.AddSingleton<ILoggerAdapter, SerilogAdapter>();
        
        return services;
    }
}