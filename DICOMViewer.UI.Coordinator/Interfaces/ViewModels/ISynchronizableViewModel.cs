using DICOMViewer.UI.Coordinator.Models;

namespace DICOMViewer.UI.Coordinator.Interfaces.ViewModels
{
    public interface ISynchronizableViewModel
    {
        void HandleSynchronizationEvent(SyncParameters parameters);
    }
}