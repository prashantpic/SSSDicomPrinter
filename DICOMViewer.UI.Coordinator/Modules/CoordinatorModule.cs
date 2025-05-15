using DICOMViewer.UI.Coordinator.Interfaces.Repositories;
using DICOMViewer.UI.Coordinator.Interfaces.Services;
using DICOMViewer.UI.Coordinator.Repositories;
using DICOMViewer.UI.Coordinator.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace DICOMViewer.UI.Coordinator.Modules
{
    public class CoordinatorModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var navigationService = containerProvider.Resolve<IApplicationNavigationService>();
            navigationService.NavigateAsync("MainContentRegion", "DefaultView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationNavigationService, ApplicationNavigationService>();
            containerRegistry.RegisterSingleton<IThemeManagementService, ThemeManagementService>();
            containerRegistry.RegisterSingleton<ILocalizationService, LocalizationService>();
            containerRegistry.RegisterSingleton<IViewStateManagementService, ViewStateManagementService>();
            containerRegistry.RegisterSingleton<IViewStateRepository, FileSystemViewStateRepository>();
            containerRegistry.RegisterSingleton<ShellViewModel>();
        }
    }
}