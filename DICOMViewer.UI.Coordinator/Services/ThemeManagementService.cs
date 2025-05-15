using Prism.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class ThemeManagementService : IThemeManagementService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IViewStateManagementService _viewStateService;
        private ResourceDictionary _currentThemeDictionary;
        private ResourceDictionary _highContrastDictionary;

        public ThemeManagementService(
            IEventAggregator eventAggregator,
            IViewStateManagementService viewStateService)
        {
            _eventAggregator = eventAggregator;
            _viewStateService = viewStateService;
        }

        public ThemeType GetCurrentTheme()
        {
            return Application.Current.Resources.MergedDictionaries
                .OfType<ResourceDictionary>()
                .First(rd => rd.Source?.ToString().Contains("Theme.") ?? false)
                .Source.ToString().Contains("Dark") ? ThemeType.Dark : ThemeType.Light;
        }

        public bool IsHighContrastActive()
        {
            return _highContrastDictionary != null;
        }

        public async Task SetThemeAsync(ThemeType themeType)
        {
            var themeFile = themeType switch
            {
                ThemeType.Dark => "Theme.Dark.xaml",
                ThemeType.Light => "Theme.Light.xaml",
                _ => "Theme.Light.xaml"
            };

            var newTheme = new ResourceDictionary { Source = new System.Uri($"/Resources/Styles/{themeFile}", System.UriKind.Relative) };

            Application.Current.Resources.MergedDictionaries.Remove(_currentThemeDictionary);
            Application.Current.Resources.MergedDictionaries.Add(newTheme);
            _currentThemeDictionary = newTheme;

            _eventAggregator.GetEvent<ThemeChangedEvent>().Publish(themeType);
            await SaveCurrentThemeSettings();
        }

        public async Task SetHighContrastModeAsync(bool isActive)
        {
            if (isActive)
            {
                _highContrastDictionary = new ResourceDictionary { Source = new System.Uri("/Resources/Styles/Theme.HighContrast.xaml", System.UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(_highContrastDictionary);
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Remove(_highContrastDictionary);
                _highContrastDictionary = null;
            }

            _eventAggregator.GetEvent<HighContrastModeChangedEvent>().Publish(isActive);
            await SaveCurrentThemeSettings();
        }

        private async Task SaveCurrentThemeSettings()
        {
            var settings = new ThemeSettings
            {
                CurrentTheme = GetCurrentTheme(),
                IsHighContrastActive = IsHighContrastActive()
            };

            await _viewStateService.SaveApplicationStateAsync(new ApplicationSettings
            {
                Theme = settings
            });
        }
    }
}