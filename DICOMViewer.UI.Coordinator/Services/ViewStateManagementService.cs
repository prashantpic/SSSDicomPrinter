using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Repositories;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;
using TheSSS.DICOMViewer.Common.Interfaces;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class ViewStateManagementService : IViewStateManagementService
    {
        private readonly IViewStateRepository _repository;
        private readonly ILoggerAdapter _logger;

        public ViewStateManagementService(IViewStateRepository repository, ILoggerAdapter logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task SaveStateAsync(string viewIdentifier, ViewState state)
            => _repository.SaveStateAsync(viewIdentifier, state);

        public Task<ViewState> LoadStateAsync(string viewIdentifier)
            => _repository.LoadStateAsync(viewIdentifier);

        public Task ClearStateAsync(string viewIdentifier)
            => _repository.ClearStateAsync(viewIdentifier);

        public Task SaveApplicationStateAsync(ApplicationSettings settings)
            => _repository.SaveApplicationSettingsAsync(settings);

        public Task<ApplicationSettings> LoadApplicationStateAsync()
            => _repository.LoadApplicationSettingsAsync();
    }
}