using DICOMViewer.UI.Coordinator.Constants;

namespace DICOMViewer.UI.Coordinator.Models
{
    public class ThemeSettings
    {
        public ThemeType CurrentTheme { get; set; }
        public bool IsHighContrastActive { get; set; }
    }
}