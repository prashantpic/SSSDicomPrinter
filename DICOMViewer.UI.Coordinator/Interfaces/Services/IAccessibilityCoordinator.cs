namespace TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services
{
    public interface IAccessibilityCoordinator
    {
        bool IsHighContrastModeActive();
        void SetHighContrastMode(bool isActive);
        void NotifyHighContrastChanged(bool isActive);
    }
}