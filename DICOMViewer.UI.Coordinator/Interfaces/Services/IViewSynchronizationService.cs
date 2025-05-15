using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services
{
    public interface IViewSynchronizationService
    {
        void RequestSynchronization(SyncParameters parameters);
        void SetSynchronizationActive(bool isActive);
        bool IsSynchronizationActive();
    }
}