using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DICOMViewer.Presentation.Services;

namespace TheSSS.DICOMViewer.Presentation.ViewModels
{
    public partial class MainApplicationWindowViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly IUserDialogService _userDialogService;
        private readonly ILocalizationService _localizationService;
        private readonly IThemeManagerService _themeManagerService;

        [ObservableProperty]
        private ObservableObject _currentMainViewModel;

        [ObservableProperty]
        private string _title = "DICOM Viewer";

        public MainApplicationWindowViewModel(
            INavigationService navigationService,
            IUserDialogService userDialogService,
            ILocalizationService localizationService,
            IThemeManagerService themeManagerService)
        {
            _navigationService = navigationService;
            _userDialogService = userDialogService;
            _localizationService = localizationService;
            _themeManagerService = themeManagerService;
        }

        [RelayCommand]
        private void OpenSettingsDialog()
        {
            _userDialogService.ShowDialogAsync(App.ServiceProvider.GetService<SettingsWindowViewModel>());
        }
    }
}