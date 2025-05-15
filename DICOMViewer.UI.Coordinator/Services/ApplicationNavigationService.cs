using Prism.Regions;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class ApplicationNavigationService : IApplicationNavigationService
    {
        private readonly IRegionManager _regionManager;

        public ApplicationNavigationService(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public Task<bool> NavigateAsync(string regionName, string viewName)
        {
            return NavigateAsync(regionName, viewName, null);
        }

        public Task<bool> NavigateAsync(string regionName, string viewName, object navigationParameters)
        {
            var result = _regionManager.RequestNavigate(
                regionName, 
                viewName, 
                new NavigationParameters
                {
                    { "navigationParameters", navigationParameters }
                });

            return Task.FromResult(result.Result);
        }
    }
}