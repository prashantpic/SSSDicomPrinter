using Prism.Regions;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Common.Interfaces;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class ApplicationNavigationService : IApplicationNavigationService
    {
        private readonly IRegionManager _regionManager;
        private readonly ILoggerAdapter _logger;

        public ApplicationNavigationService(IRegionManager regionManager, ILoggerAdapter logger)
        {
            _regionManager = regionManager;
            _logger = logger;
        }

        public Task<bool> NavigateAsync(string regionName, string viewName)
            => NavigateAsync(regionName, viewName, null);

        public Task<bool> NavigateAsync(string regionName, string viewName, object navigationParameters)
        {
            var tcs = new TaskCompletionSource<bool>();
            _regionManager.RequestNavigate(regionName, viewName, result =>
            {
                if (result.Error != null)
                    _logger.Error(result.Error, $"Navigation failed to '{viewName}'");
                tcs.SetResult(result.Result ?? false);
            }, navigationParameters);
            return tcs.Task;
        }
    }
}