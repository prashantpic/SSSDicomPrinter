using DICOMViewer.UI.Coordinator.Constants;
using System.Threading.Tasks;

namespace DICOMViewer.UI.Coordinator.Interfaces.Services
{
    public interface IThemeManagementService
    {
        ThemeType GetCurrentTheme();
        bool IsHighContrastActive();
        Task SetThemeAsync(ThemeType themeType);
        Task SetHighContrastModeAsync(bool isActive);
    }
}