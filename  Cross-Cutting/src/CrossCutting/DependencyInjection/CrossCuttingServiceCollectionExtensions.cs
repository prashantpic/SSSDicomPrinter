using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using TheSSS.DicomViewer.Common.Configuration;
using TheSSS.DicomViewer.Common.Configuration.Secure;
using TheSSS.DicomViewer.Common.Localization;
using TheSSS.DicomViewer.Common.Logging;
using TheSSS.DicomViewer.Common.Logging.Serilog;
using TheSSS.DicomViewer.Common.Logging.Serilog.Phi;

namespace TheSSS.DicomViewer.Common.DependencyInjection;

public static class CrossCuttingServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PhiScrubberOptions>()
            .Bind(configuration.GetSection("Logging:PhiScrubberOptions"));
        
        services.AddOptions<DpapiDataProtectorOptions>()
            .Bind(configuration.GetSection("Configuration:DpapiDataProtectorOptions"));

        services.AddScoped<Logging.Phi.IPhiScrubber, RegexPhiScrubber>();
        services.AddSingleton<ISecureDataProtector, DpapiDataProtector>();
        services.AddSingleton<IAppConfigurationService, AppConfigurationService>();

        services.AddSingleton<ILocalizationManager>(sp =>
            new ResourceManagerLocalizationManager(
                typeof(CrossCuttingServiceCollectionExtensions),
                "TheSSS.DicomViewer.Common.Localization.Resources.Strings"));

        var phiScrubber = services.BuildServiceProvider().GetRequiredService<Logging.Phi.IPhiScrubber>();
        var logger = SerilogConfigurator.CreateLogger(configuration, phiScrubber);
        services.AddSingleton<ILogger>(logger);
        services.AddSingleton<ILoggerAdapter, SerilogAdapter>();

        return services;
    }
}