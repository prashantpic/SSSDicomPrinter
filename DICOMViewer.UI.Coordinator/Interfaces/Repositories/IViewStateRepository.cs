using DICOMViewer.UI.Coordinator.Models;
using System.Threading.Tasks;

namespace DICOMViewer.UI.Coordinator.Interfaces.Repositories
{
    public interface IViewStateRepository
    {
        Task SaveStateAsync(string viewKey, ViewState viewState);
        Task<ViewState> LoadStateAsync(string viewKey);
    }
}