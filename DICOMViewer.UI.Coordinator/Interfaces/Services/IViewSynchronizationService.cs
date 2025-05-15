using DICOMViewer.UI.Coordinator.Models;

namespace DICOMViewer.UI.Coordinator.Interfaces.Services
{
    public interface IViewSynchronizationService
    {
        void RequestSynchronization(SyncParameters parameters);
        void SetSynchronizationActive(bool isActive);
        bool IsSynchronizationActive();
    }
}