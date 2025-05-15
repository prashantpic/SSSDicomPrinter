using Prism.Events;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IViewStateManagementService _viewStateService;
        private ResourceDictionary _currentLanguageDictionary;

        public LocalizationService(
            IEventAggregator eventAggregator,
            IViewStateManagementService viewStateService)
        {
            _eventAggregator = eventAggregator;
            _viewStateService = viewStateService;
        }

        public LanguageCode GetCurrentLanguage()
        {
            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() switch
            {
                "ES" => LanguageCode.ES,
                _ => LanguageCode.EN
            };
        }

        public string GetString(string key)
        {
            return Application.Current.TryFindResource(key) as string ?? key;
        }

        public async Task SetLanguageAsync(LanguageCode languageCode)
        {
            var culture = new CultureInfo(languageCode.ToString().ToLower());
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var languageFile = $"Strings.{languageCode}.xaml";
            var newLanguage = new ResourceDictionary { Source = new System.Uri($"/Resources/Localization/{languageFile}", System.UriKind.Relative) };

            Application.Current.Resources.MergedDictionaries.Remove(_currentLanguageDictionary);
            Application.Current.Resources.MergedDictionaries.Add(newLanguage);
            _currentLanguageDictionary = newLanguage;

            _eventAggregator.GetEvent<LanguageChangedEvent>().Publish(languageCode);
            await SaveLanguageSettings(languageCode);
        }

        private async Task SaveLanguageSettings(LanguageCode languageCode)
        {
            await _viewStateService.SaveApplicationStateAsync(new ApplicationSettings
            {
                Language = new LanguageSetting { SelectedLanguage = languageCode }
            });
        }
    }
}