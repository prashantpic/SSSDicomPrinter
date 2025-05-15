using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DICOMViewer.UI.Coordinator.Constants;
using DICOMViewer.UI.Coordinator.Events;
using DICOMViewer.UI.Coordinator.Interfaces.Services;
using Prism.Events;
using System.Threading.Tasks;

namespace DICOMViewer.UI.Coordinator.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IThemeManagementService _themeService;
        private readonly ILocalizationService _localizationService;

        [ObservableProperty]
        private string _title = "DICOM Viewer";

        [ObservableProperty]
        private ThemeType _currentTheme;

        [ObservableProperty]
        private LanguageCode _currentLanguage;

        [RelayCommand]
        private void ExitApplication() => Application.Current.Shutdown();

        public ShellViewModel(
            IEventAggregator eventAggregator,
            IThemeManagementService themeService,
            ILocalizationService localizationService,
            IViewStateManagementService viewStateService,
            IApplicationNavigationService navigationService)
        {
            _eventAggregator = eventAggregator;
            _themeService = themeService;
            _localizationService = localizationService;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(HandleThemeChanged);
            _eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(HandleLanguageChanged);
        }

        private void HandleThemeChanged(ThemeType newTheme) => CurrentTheme = newTheme;
        private void HandleLanguageChanged(LanguageCode newLanguage) => CurrentLanguage = newLanguage;

        public async Task InitializeAsync()
        {
            CurrentTheme = _themeService.GetCurrentTheme();
            CurrentLanguage = _localizationService.GetCurrentLanguage();
        }
    }
}