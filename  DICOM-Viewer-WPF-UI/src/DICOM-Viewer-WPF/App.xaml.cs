using Microsoft.Extensions.DependencyInjection;
using TheSSS.DicomViewer.Presentation.ViewModels;
using TheSSS.DicomViewer.Presentation.Services;
using System.Windows;

namespace TheSSS.DicomViewer.Presentation
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            
            services.AddTransient<IncomingPrintQueueTabViewModel>();
            services.AddTransient<LocalStorageTabViewModel>();
            services.AddTransient<QueryRetrieveTabViewModel>();
            
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IThemeManager, ThemeManager>();
            services.AddSingleton<IRenderer<SKCanvas, DicomImageViewModel, SKRect>, DicomPixelDataRenderer>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }
    }
}