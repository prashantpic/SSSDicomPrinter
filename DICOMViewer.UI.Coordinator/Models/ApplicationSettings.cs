namespace DICOMViewer.UI.Coordinator.Models
{
    public class ApplicationSettings
    {
        public ThemeSettings Theme { get; set; }
        public LanguageSetting Language { get; set; }
        public Dictionary<string, ViewState> ViewStates { get; set; } = new Dictionary<string, ViewState>();
    }
}