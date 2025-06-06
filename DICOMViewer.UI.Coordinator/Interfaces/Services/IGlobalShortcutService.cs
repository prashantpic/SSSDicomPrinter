using System.Windows.Input;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services
{
    public interface IGlobalShortcutService
    {
        void RegisterShortcut(KeyGesture keyGesture, Action<object> callback, object callbackParameter = null);
        void UnregisterShortcut(KeyGesture keyGesture);
        void StartListening();
        void StopListening();
    }
}