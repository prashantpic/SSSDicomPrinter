using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services
{
    public interface IViewStateManagementService
    {
        Task SaveStateAsync(string viewIdentifier, ViewState state);
        Task<ViewState> LoadStateAsync(string viewIdentifier);
        Task ClearStateAsync(string viewIdentifier);
        Task SaveApplicationStateAsync(ApplicationSettings settings);
        Task<ApplicationSettings> LoadApplicationStateAsync();
    }
}