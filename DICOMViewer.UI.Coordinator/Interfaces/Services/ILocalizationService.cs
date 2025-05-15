using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services
{
    public interface ILocalizationService
    {
        LanguageCode GetCurrentLanguage();
        Task SetLanguageAsync(LanguageCode languageCode);
        string GetString(string key);
    }
}