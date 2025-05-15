using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using TheSSS.DICOMViewer.Presentation.Models;

namespace TheSSS.DICOMViewer.Presentation.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IAppSettingsService _appSettingsService;

        public LocalizationService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public IEnumerable<LanguageItem> GetAvailableLanguages()
        {
            return new List<LanguageItem>
            {
                new() { DisplayName = "English", CultureCode = "en-US" },
                new() { DisplayName = "Español", CultureCode = "es-ES" }
            };
        }

        public string GetString(string key)
        {
            return Application.Current.TryFindResource(key) as string ?? key;
        }

        public async Task SetLanguageAsync(string cultureCode)
        {
            // Implementation for setting language
        }
    }
}