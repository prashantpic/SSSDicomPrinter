using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DicomViewer.Presentation.Services;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Settings.Panels
{
    public partial class DisplaySettingsPanelViewModel : ObservableObject
    {
        private readonly IThemeManager _themeManager;

        [ObservableProperty]
        private string _selectedTheme;

        [ObservableProperty]
        private bool _highContrastEnabled;

        public DisplaySettingsPanelViewModel(IThemeManager themeManager)
        {
            _themeManager = themeManager;
            SelectedTheme = _themeManager.GetCurrentTheme();
            ApplyThemeCommand = new RelayCommand(ApplyTheme);
        }

        [RelayCommand]
        private void ApplyTheme()
        {
            _themeManager.SetTheme(SelectedTheme);
        }

        public RelayCommand ApplyThemeCommand { get; }
        
        public List<string> AvailableThemes => new List<string> { "Light", "Dark" };
    }
}