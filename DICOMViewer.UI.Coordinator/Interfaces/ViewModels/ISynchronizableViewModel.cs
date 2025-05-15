using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.ViewModels
{
    public interface ISynchronizableViewModel
    {
        void HandleSynchronizationEvent(SyncParameters parameters);
    }
}