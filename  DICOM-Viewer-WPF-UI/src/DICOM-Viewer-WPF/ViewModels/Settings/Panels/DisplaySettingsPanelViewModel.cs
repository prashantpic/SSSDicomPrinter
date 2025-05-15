using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DicomViewer.Presentation.Services;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Settings.Panels
{
    public partial class DisplaySettingsPanelViewModel : ObservableObject
    {
        private readonly IThemeManager _themeManager;

        [ObservableProperty]
        private string? _selectedTheme;

        public DisplaySettingsPanelViewModel(IThemeManager themeManager)
        {
            _themeManager = themeManager;
        }

        [RelayCommand]
        private void ApplyTheme()
        {
            if (!string.IsNullOrEmpty(SelectedTheme))
            {
                _themeManager.SetTheme(SelectedTheme);
            }
        }
    }
}