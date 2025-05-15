using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using DICOMViewer.UI.Coordinator.ViewModels;
using DICOMViewer.UI.Coordinator.Interfaces.Services;
using DICOMViewer.UI.Coordinator.Services;

namespace DICOMViewer.UI.Coordinator
{
    public class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IEventAggregator, Prism.Events.EventAggregator>();
            containerRegistry.RegisterSingleton<IRegionManager, Prism.Regions.RegionManager>();
            
            containerRegistry.RegisterSingleton<IApplicationNavigationService, ApplicationNavigationService>();
            containerRegistry.RegisterSingleton<IThemeManagementService, ThemeManagementService>();
            containerRegistry.RegisterSingleton<ILocalizationService, LocalizationService>();
            containerRegistry.RegisterSingleton<IViewStateManagementService, ViewStateManagementService>();
            containerRegistry.RegisterSingleton<IAccessibilityCoordinator, AccessibilityCoordinatorService>();
            
            containerRegistry.RegisterSingleton<ShellViewModel>();
            containerRegistry.RegisterSingleton<ShellWindow>();
        }

        protected override void InitializeShell(DependencyObject shell)
        {
            base.InitializeShell(shell);
            Application.Current.MainWindow = (Window)shell;
            Application.Current.MainWindow.Show();
        }
    }
}