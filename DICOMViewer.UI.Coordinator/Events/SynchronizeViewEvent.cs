using Prism.Events;
using DICOMViewer.UI.Coordinator.Models;

namespace DICOMViewer.UI.Coordinator.Events
{
    public class SynchronizeViewEvent : PubSubEvent<SyncParameters> { }
}