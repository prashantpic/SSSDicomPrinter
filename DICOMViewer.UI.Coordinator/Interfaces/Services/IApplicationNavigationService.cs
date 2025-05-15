using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services
{
    public interface IApplicationNavigationService
    {
        Task<bool> NavigateAsync(string regionName, string viewName);
        Task<bool> NavigateAsync(string regionName, string viewName, object navigationParameters);
    }
}