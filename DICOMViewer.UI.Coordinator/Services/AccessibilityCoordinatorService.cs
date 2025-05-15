using Prism.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class AccessibilityCoordinatorService : IAccessibilityCoordinator
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IThemeManagementService _themeService;

        public AccessibilityCoordinatorService(IEventAggregator eventAggregator, IThemeManagementService themeService)
        {
            _eventAggregator = eventAggregator;
            _themeService = themeService;
        }

        public bool IsHighContrastModeActive() => _themeService.IsHighContrastActive();
        public void SetHighContrastMode(bool isActive) => _themeService.SetHighContrastModeAsync(isActive);
        public void NotifyHighContrastChanged(bool isActive) => _eventAggregator.GetEvent<HighContrastModeChangedEvent>().Publish(isActive);
    }
}