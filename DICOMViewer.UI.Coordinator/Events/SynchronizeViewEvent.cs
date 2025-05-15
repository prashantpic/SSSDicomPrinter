using Prism.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Events
{
    public class SynchronizeViewEvent : PubSubEvent<SyncParameters> { }
}