using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;
using TheSSS.DICOMViewer.Presentation.Coordinator.Views;
using TheSSS.DICOMViewer.Presentation.Coordinator.Modules;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Presentation.Coordinator.Services;

namespace TheSSS.DICOMViewer.Presentation.Coordinator
{
    public class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationNavigationService, ApplicationNavigationService>();
            containerRegistry.RegisterSingleton<IThemeManagementService, ThemeManagementService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<CoordinatorModule>();
        }

        protected override void InitializeShell(DependencyObject shell)
        {
            Application.Current.MainWindow = shell as Window;
            Application.Current.MainWindow.Show();
        }
    }
}