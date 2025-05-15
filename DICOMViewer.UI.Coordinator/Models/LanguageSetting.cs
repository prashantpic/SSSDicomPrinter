using System.Text.Json.Serialization;
using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Models
{
    public class LanguageSetting
    {
        [JsonPropertyName("selectedLanguage")]
        public LanguageCode SelectedLanguage { get; set; }
    }
}