using System.Threading.Tasks;

namespace DICOMViewer.UI.Coordinator.Interfaces.Services
{
    public interface IApplicationNavigationService
    {
        Task<bool> NavigateAsync(string regionName, string viewName);
        Task<bool> NavigateAsync(string regionName, string viewName, object navigationParameters);
    }
}