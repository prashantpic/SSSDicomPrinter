using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Repositories
{
    public interface IViewStateRepository
    {
        Task SaveStateAsync(string viewKey, ViewState viewState);
        Task<ViewState> LoadStateAsync(string viewKey);
        Task ClearStateAsync(string viewKey);
        Task SaveApplicationSettingsAsync(ApplicationSettings settings);
        Task<ApplicationSettings> LoadApplicationSettingsAsync();
    }
}