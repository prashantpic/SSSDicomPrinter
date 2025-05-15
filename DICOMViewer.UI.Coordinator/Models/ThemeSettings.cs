using System.Text.Json.Serialization;
using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Models
{
    public class ThemeSettings
    {
        [JsonPropertyName("currentTheme")]
        public ThemeType CurrentTheme { get; set; }
        
        [JsonPropertyName("isHighContrastActive")]
        public bool IsHighContrastActive { get; set; }
    }
}