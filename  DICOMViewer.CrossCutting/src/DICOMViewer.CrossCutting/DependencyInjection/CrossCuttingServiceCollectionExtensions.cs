namespace TheSSS.DICOMViewer.Common.DependencyInjection
{
    public static class CrossCuttingServiceCollectionExtensions
    {
        public static IServiceCollection AddCrossCuttingServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredSerilog(configuration)
                .AddSingleton<TheSSS.DICOMViewer.Common.Abstractions.Security.IDataProtectionProvider, Security.DpapiDataProtectionProvider>()
                .AddSingleton<TheSSS.DICOMViewer.Common.Abstractions.Configuration.IAppConfiguration, Configuration.AppConfiguration>();

            return services;
        }
    }
}