namespace DICOMViewer.UI.Coordinator.Interfaces.Services
{
    public interface IAccessibilityCoordinator
    {
        bool IsHighContrastModeActive();
        void SetHighContrastMode(bool isActive);
    }
}