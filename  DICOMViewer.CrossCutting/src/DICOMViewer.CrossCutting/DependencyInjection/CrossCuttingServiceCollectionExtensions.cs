namespace TheSSS.DICOMViewer.Common.DependencyInjection;

public static class CrossCuttingServiceCollectionExtensions
{
    public static IServiceCollection AddCrossCuttingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfiguredSerilog(configuration)
            .AddSingleton<IDataProtectionProvider, DpapiDataProtectionProvider>()
            .AddSingleton<IAppConfiguration, AppConfiguration>();

        return services;
    }
}