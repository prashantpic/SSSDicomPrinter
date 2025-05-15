using Prism.Events;
using System.Windows;
using System.Globalization;
using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Common.Interfaces;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IEventAggregator _eventAggregator;
        private LanguageCode _currentLanguage = LanguageCode.EN;

        public LocalizationService(IEventAggregator eventAggregator, ILoggerAdapter logger)
        {
            _eventAggregator = eventAggregator;
        }

        public LanguageCode GetCurrentLanguage() => _currentLanguage;

        public async Task SetLanguageAsync(LanguageCode languageCode)
        {
            _currentLanguage = languageCode;
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary {
                Source = new Uri($"/TheSSS.DICOMViewer.Presentation.Coordinator;component/Resources/Localization/Strings.{languageCode}.xaml", UriKind.Relative)
            });
            _eventAggregator.GetEvent<LanguageChangedEvent>().Publish(_currentLanguage);
        }

        public string GetString(string key) => Application.Current.TryFindResource(key) as string ?? key;
    }
}