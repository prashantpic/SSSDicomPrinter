using DICOMViewer.UI.Coordinator.Models;
using System.Threading.Tasks;

namespace DICOMViewer.UI.Coordinator.Interfaces.Services
{
    public interface IViewStateManagementService
    {
        Task SaveApplicationStateAsync(ApplicationSettings settings);
        Task<ApplicationSettings> LoadApplicationStateAsync();
        Task SaveStateAsync(string viewIdentifier, ViewState state);
        Task<ViewState> LoadStateAsync(string viewIdentifier);
        Task ClearStateAsync(string viewIdentifier);
    }
}