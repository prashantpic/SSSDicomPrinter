using Prism.Ioc;
using Prism.Modularity;
using TheSSS.DICOMViewer.Presentation.Coordinator.Infrastructure.Persistence;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Repositories;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Presentation.Coordinator.Services;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Modules
{
    public class CoordinatorModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<IGlobalShortcutService>().StartListening();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationNavigationService, ApplicationNavigationService>();
            containerRegistry.RegisterSingleton<IThemeManagementService, ThemeManagementService>();
            containerRegistry.RegisterSingleton<IViewStateManagementService, ViewStateManagementService>();
            containerRegistry.RegisterSingleton<IViewSynchronizationService, ViewSynchronizationService>();
            containerRegistry.RegisterSingleton<ILocalizationService, LocalizationService>();
            containerRegistry.RegisterSingleton<IGlobalShortcutService, GlobalShortcutService>();
            containerRegistry.RegisterSingleton<IAccessibilityCoordinator, AccessibilityCoordinatorService>();
            containerRegistry.RegisterSingleton<IViewStateRepository, FileSystemViewStateRepository>();
        }
    }
}