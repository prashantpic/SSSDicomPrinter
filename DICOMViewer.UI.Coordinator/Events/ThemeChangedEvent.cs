using Prism.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Events
{
    public class ThemeChangedEvent : PubSubEvent<ThemeType> { }
}