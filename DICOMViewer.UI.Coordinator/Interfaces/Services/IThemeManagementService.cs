using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services
{
    public interface IThemeManagementService
    {
        ThemeType GetCurrentTheme();
        Task SetThemeAsync(ThemeType themeType);
        bool IsHighContrastActive();
        Task SetHighContrastModeAsync(bool isActive);
        Task LoadThemeSettingsAsync();
    }
}