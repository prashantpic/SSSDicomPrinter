using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Repositories;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class ViewStateManagementService : IViewStateManagementService
    {
        private readonly IViewStateRepository _repository;
        private ApplicationSettings _applicationSettings = new();

        public ViewStateManagementService(IViewStateRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveApplicationStateAsync(ApplicationSettings settings)
        {
            _applicationSettings = settings;
            await _repository.SaveStateAsync("ApplicationSettings", settings);
        }

        public async Task<ApplicationSettings?> LoadApplicationStateAsync()
        {
            _applicationSettings = await _repository.LoadStateAsync("ApplicationSettings") ?? new ApplicationSettings();
            return _applicationSettings;
        }

        public async Task SaveStateAsync(string viewIdentifier, ViewState state)
        {
            _applicationSettings.ViewStates ??= new Dictionary<string, ViewState>();
            _applicationSettings.ViewStates[viewIdentifier] = state;
            await SaveApplicationStateAsync(_applicationSettings);
        }

        public async Task<ViewState?> LoadStateAsync(string viewIdentifier)
        {
            await LoadApplicationStateAsync();
            return _applicationSettings.ViewStates?.TryGetValue(viewIdentifier, out var state) == true ? state : null;
        }

        public async Task ClearStateAsync(string viewIdentifier)
        {
            if (_applicationSettings.ViewStates?.ContainsKey(viewIdentifier) == true)
            {
                _applicationSettings.ViewStates.Remove(viewIdentifier);
                await SaveApplicationStateAsync(_applicationSettings);
            }
        }
    }
}