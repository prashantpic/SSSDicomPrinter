using CommunityToolkit.Mvvm.ComponentModel;
using TheSSS.DicomViewer.Presentation.Services;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Settings.Panels
{
    public partial class DisplaySettingsPanelViewModel : ObservableObject
    {
        private readonly IThemeManager _themeManager;

        [ObservableProperty]
        private string _selectedTheme;

        public ObservableCollection<string> AvailableThemes { get; } = new() { "Light", "Dark" };

        public DisplaySettingsPanelViewModel(IThemeManager themeManager)
        {
            _themeManager = themeManager;
            SelectedTheme = _themeManager.GetCurrentTheme();
        }

        partial void OnSelectedThemeChanged(string value)
        {
            _themeManager.SetTheme(value);
        }
    }
}