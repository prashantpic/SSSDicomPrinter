using Prism.Events;
using System.Windows;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Common.Interfaces;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class ThemeManagementService : IThemeManagementService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IViewStateRepository _settingsRepository;
        private readonly ILoggerAdapter _logger;
        private ThemeType _currentTheme = ThemeType.Light;

        public ThemeManagementService(IEventAggregator eventAggregator, IViewStateRepository settingsRepository, ILoggerAdapter logger)
        {
            _eventAggregator = eventAggregator;
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        public ThemeType GetCurrentTheme() => _currentTheme;

        public async Task SetThemeAsync(ThemeType themeType)
        {
            if (_currentTheme == themeType) return;
            
            _currentTheme = themeType;
            ApplyThemeResources(themeType);
            _eventAggregator.GetEvent<ThemeChangedEvent>().Publish(_currentTheme);
            await SaveThemeSettingsAsync();
        }

        private void ApplyThemeResources(ThemeType themeType)
        {
            var appResources = Application.Current.Resources;
            appResources.MergedDictionaries.Clear();
            
            var themePath = themeType switch {
                ThemeType.Dark => "Theme.Dark.xaml",
                ThemeType.HighContrast => "Theme.HighContrast.xaml",
                _ => "Theme.Light.xaml"
            };
            
            appResources.MergedDictionaries.Add(new ResourceDictionary {
                Source = new Uri($"/TheSSS.DICOMViewer.Presentation.Coordinator;component/Resources/Styles/{themePath}", UriKind.Relative)
            });
        }

        private async Task SaveThemeSettingsAsync()
        {
            var settings = await _settingsRepository.LoadApplicationSettingsAsync() ?? new ApplicationSettings();
            settings.CurrentThemeSettings.CurrentTheme = _currentTheme;
            await _settingsRepository.SaveApplicationSettingsAsync(settings);
        }

        public Task LoadThemeSettingsAsync() => Task.CompletedTask;
        public bool IsHighContrastActive() => _currentTheme == ThemeType.HighContrast;
        public Task SetHighContrastModeAsync(bool isActive) => SetThemeAsync(isActive ? ThemeType.HighContrast : ThemeType.Light);
    }
}