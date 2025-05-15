using System.Collections.Generic;
using TheSSS.DICOMViewer.Presentation.Models;

namespace TheSSS.DICOMViewer.Presentation.Services
{
    public interface ILocalizationService
    {
        IEnumerable<LanguageItem> GetAvailableLanguages();
        string GetString(string key);
        Task SetLanguageAsync(string cultureCode);
    }
}