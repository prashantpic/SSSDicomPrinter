using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TheSSS.DicomViewer.Presentation.Services;
using TheSSS.DicomViewer.Presentation.ViewModels;
using TheSSS.DicomViewer.Presentation.ViewModels.Tabs;

namespace TheSSS.DicomViewer.Presentation;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var services = new ServiceCollection();
        ConfigureServices(services);
        
        _serviceProvider = services.BuildServiceProvider();
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IThemeManager, ThemeManager>();
        
        services.AddTransient<IncomingPrintQueueTabViewModel>();
        services.AddTransient<LocalStorageTabViewModel>();
        services.AddTransient<QueryRetrieveTabViewModel>();
        
        services.AddTransient<DicomImageViewModel>();
        services.AddTransient<ThumbnailGridViewModel>();
        services.AddTransient<ThumbnailViewModel>();
        
        services.AddSingleton<IRenderer, DicomPixelDataRenderer>();
        
        services.AddTransient<SettingsShellViewModel>();
        services.AddTransient<DisplaySettingsPanelViewModel>();
    }
}