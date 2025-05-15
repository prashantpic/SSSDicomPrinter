using Prism.Events;
using DICOMViewer.UI.Coordinator.Constants;

namespace DICOMViewer.UI.Coordinator.Events
{
    public class LanguageChangedEvent : PubSubEvent<LanguageCode> { }
}