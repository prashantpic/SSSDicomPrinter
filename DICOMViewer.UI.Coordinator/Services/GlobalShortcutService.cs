using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Interop;
using Prism.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class GlobalShortcutService : IGlobalShortcutService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Dictionary<KeyGesture, Action<object?>> _shortcuts = new();
        private HwndSource? _source;

        public GlobalShortcutService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void RegisterShortcut(KeyGesture keyGesture, Action<object?> callback, object? callbackParameter = null)
        {
            _shortcuts[keyGesture] = callback;
        }

        public void UnregisterShortcut(KeyGesture keyGesture)
        {
            _shortcuts.Remove(keyGesture);
        }

        public void StartListening()
        {
            var window = Application.Current.MainWindow;
            if (window == null) return;

            var handle = new WindowInteropHelper(window).Handle;
            _source = HwndSource.FromHwnd(handle);
            _source?.AddHook(HwndHook);
        }

        public void StopListening()
        {
            _source?.RemoveHook(HwndHook);
            _source = null;
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0100) // WM_KEYDOWN
            {
                var key = KeyInterop.KeyFromVirtualKey((int)wParam);
                foreach (var (gesture, callback) in _shortcuts)
                {
                    if (gesture.Key == key && Keyboard.Modifiers == gesture.Modifiers)
                    {
                        callback(null);
                        _eventAggregator.GetEvent<GlobalShortcutActivatedEvent>()
                            .Publish(gesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture));
                        handled = true;
                        break;
                    }
                }
            }
            return IntPtr.Zero;
        }
    }
}