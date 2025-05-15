using System.Text.Json.Serialization;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Models
{
    public class ApplicationSettings
    {
        [JsonPropertyName("themeSettings")]
        public ThemeSettings ThemeSettings { get; set; } = new ThemeSettings();
        
        [JsonPropertyName("languageSetting")]
        public LanguageSetting LanguageSetting { get; set; } = new LanguageSetting();
    }
}