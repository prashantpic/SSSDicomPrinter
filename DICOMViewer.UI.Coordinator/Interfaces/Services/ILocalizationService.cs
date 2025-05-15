using DICOMViewer.UI.Coordinator.Constants;

namespace DICOMViewer.UI.Coordinator.Interfaces.Services
{
    public interface ILocalizationService
    {
        LanguageCode GetCurrentLanguage();
        string GetString(string key);
        Task SetLanguageAsync(LanguageCode languageCode);
    }
}