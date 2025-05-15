using System.Windows.Input;
using Prism.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Common.Interfaces;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class GlobalShortcutService : IGlobalShortcutService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Dictionary<KeyGesture, Action<object>> _shortcuts = new();

        public GlobalShortcutService(IEventAggregator eventAggregator, ILoggerAdapter logger)
        {
            _eventAggregator = eventAggregator;
        }

        public void RegisterShortcut(KeyGesture gesture, Action<object> callback, object parameter = null)
        {
            _shortcuts[gesture] = callback;
            InputManager.Current.PreProcessInput += HandleInput;
        }

        private void HandleInput(object sender, PreProcessInputEventArgs e)
        {
            if (e.StagingItem.Input is KeyEventArgs keyArgs && keyArgs.KeyboardDevice.IsKeyDown(keyArgs.Key))
            {
                foreach (var (gesture, action) in _shortcuts)
                {
                    if (gesture.Matches(null, keyArgs))
                    {
                        action.Invoke(null);
                        keyArgs.Handled = true;
                    }
                }
            }
        }

        public void UnregisterShortcut(KeyGesture gesture) => _shortcuts.Remove(gesture);
        public void StartListening() { }
        public void StopListening() => InputManager.Current.PreProcessInput -= HandleInput;
    }
}