namespace TheSSS.DICOMViewer.Common.Logging
{
    public static class SerilogConfigurationExtensions
    {
        public static IServiceCollection AddConfiguredSerilog(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.With<Enrichers.PhiMaskingEnricher>();

            var logger = loggerConfiguration.CreateLogger();
            services.AddSingleton<Serilog.ILogger>(logger);
            services.AddSingleton<TheSSS.DICOMViewer.Common.Abstractions.Logging.ILoggerAdapter, SerilogAdapter>();
            
            return services;
        }
    }
}