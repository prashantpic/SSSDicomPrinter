using System;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TheSSS.DICOMViewer.Presentation.Services;
using TheSSS.DICOMViewer.Presentation.ViewModels;

namespace TheSSS.DICOMViewer.Presentation
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Initialize theme and localization
            var themeService = ServiceProvider.GetRequiredService<IThemeManagerService>();
            var localizationService = ServiceProvider.GetRequiredService<ILocalizationService>();
            
            // Apply stored preferences
            themeService.ApplyThemeAsync(themeService.CurrentThemeName).Wait();
            localizationService.SetLanguageAsync(localizationService.CurrentCulture.Name).Wait();

            // Create and show main window
            var mainWindow = new Views.MainApplicationWindow();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainApplicationWindowViewModel>();
            mainWindow.Show();

            DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Presentation Layer Services
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<IThemeManagerService, ThemeManagerService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IUserDialogService, UserDialogService>();
            services.AddSingleton<IViewLocator, ViewLocator>();
            services.AddSingleton<ISkiaDicomDrawer, SkiaDicomDrawer>();

            // ViewModels
            services.AddTransient<MainApplicationWindowViewModel>();
            services.AddTransient<DicomImageViewModel>();
            services.AddTransient<SettingsWindowViewModel>();
            services.AddTransient<LocalizationSettingsPanelViewModel>();
            services.AddTransient<ThemeSettingsPanelViewModel>();

            // External Services (assumed registered in host)
            services.AddSingleton<IDicomDataProviderService>(_ => throw new NotImplementedException());
            services.AddSingleton<IAppSettingsService>(_ => throw new NotImplementedException());
            services.AddSingleton<ILoggerAdapter>(_ => throw new NotImplementedException());
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = ServiceProvider.GetService<ILoggerAdapter>();
            logger?.Error(e.Exception, "Unhandled exception occurred");
            
            MessageBox.Show(
                "A critical error occurred. The application may become unstable.",
                "Fatal Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}