using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Common.Interfaces;
using TheSSS.DICOMViewer.Presentation.Coordinator.Constants;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationNavigationService _navigationService;
        private readonly IThemeManagementService _themeService;
        private readonly ILocalizationService _localizationService;
        private readonly IViewStateManagementService _stateService;
        private readonly ILoggerAdapter _logger;

        [ObservableProperty]
        private string _windowTitle = "DICOM Viewer";

        [ObservableProperty]
        private ThemeType _currentTheme;

        [ObservableProperty]
        private LanguageCode _currentLanguage;

        public ShellViewModel(IEventAggregator eventAggregator,
            IApplicationNavigationService navigationService,
            IThemeManagementService themeService,
            ILocalizationService localizationService,
            IViewStateManagementService stateService,
            ILoggerAdapter logger)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            _themeService = themeService;
            _localizationService = localizationService;
            _stateService = stateService;
            _logger = logger;

            _eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(HandleThemeChanged);
            _eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(HandleLanguageChanged);
            
            CurrentTheme = _themeService.GetCurrentTheme();
            CurrentLanguage = _localizationService.GetCurrentLanguage();
        }

        [RelayCommand]
        private async Task InitializeAsync()
        {
            await _stateService.LoadApplicationStateAsync();
            await _navigationService.NavigateAsync(RegionNames.MainContentRegion, "DicomImageView");
        }

        private void HandleThemeChanged(ThemeType newTheme) => CurrentTheme = newTheme;
        private void HandleLanguageChanged(LanguageCode newLanguage) => CurrentLanguage = newLanguage;

        [RelayCommand]
        private void ExitApplication() => Application.Current.Shutdown();
    }
}