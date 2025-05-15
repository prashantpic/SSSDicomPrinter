using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TheSSS.DICOMViewer.Presentation.Services;
using TheSSS.DICOMViewer.Presentation.ViewModels;

namespace TheSSS.DICOMViewer.Presentation
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var services = new ServiceCollection();
            ConfigureServices(services);
            
            ServiceProvider = services.BuildServiceProvider();
            
            InitializeServices();
            ShowMainWindow();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Services
            services.AddSingleton<IThemeManagerService, ThemeManagerService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IUserDialogService, UserDialogService>();
            services.AddSingleton<IViewLocator, ViewLocator>();
            services.AddSingleton<ISkiaDicomDrawer, SkiaDicomDrawer>();
            
            // ViewModels
            services.AddSingleton<MainApplicationWindowViewModel>();
            services.AddTransient<SettingsWindowViewModel>();
            services.AddTransient<LocalizationSettingsPanelViewModel>();
            services.AddTransient<ThemeSettingsPanelViewModel>();
            
            // Cross-cutting dependencies (placeholder interfaces)
            services.AddSingleton<ILoggerAdapter, NullLoggerAdapter>();
            services.AddSingleton<IAppSettingsService, AppSettingsServiceStub>();
            services.AddSingleton<IDicomDataProviderService, DicomDataProviderStub>();
        }

        private void InitializeServices()
        {
            var themeService = ServiceProvider.GetRequiredService<IThemeManagerService>();
            themeService.ApplyThemeAsync("HighContrast").ConfigureAwait(false);
            
            var localizationService = ServiceProvider.GetRequiredService<ILocalizationService>();
            localizationService.SetLanguageAsync("en-US").ConfigureAwait(false);
        }

        private void ShowMainWindow()
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainApplicationWindowViewModel>();
            var window = new MainApplicationWindow { DataContext = mainWindow };
            window.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = ServiceProvider.GetRequiredService<ILoggerAdapter>();
            logger.Error(e.Exception, "Unhandled application error");
            e.Handled = true;
        }
    }

    // Stub implementations for cross-cutting services
    public interface ILoggerAdapter { void Error(Exception ex, string message); }
    public class NullLoggerAdapter : ILoggerAdapter { public void Error(Exception ex, string message) { } }
    public interface IAppSettingsService { }
    public class AppSettingsServiceStub : IAppSettingsService { }
    public interface IDicomDataProviderService { }
    public class DicomDataProviderStub : IDicomDataProviderService { }
}