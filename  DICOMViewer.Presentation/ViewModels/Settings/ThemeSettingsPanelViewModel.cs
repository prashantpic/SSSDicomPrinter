using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TheSSS.DICOMViewer.Presentation.Models;
using TheSSS.DICOMViewer.Presentation.Services;

namespace TheSSS.DICOMViewer.Presentation.ViewModels.Settings
{
    public partial class ThemeSettingsPanelViewModel : ObservableObject
    {
        private readonly IThemeManagerService _themeManagerService;

        [ObservableProperty]
        private ThemeItem? _selectedTheme;

        public ObservableCollection<ThemeItem> AvailableThemes { get; } = new();

        public ThemeSettingsPanelViewModel(IThemeManagerService themeManagerService)
        {
            _themeManagerService = themeManagerService;
            InitializeThemes();
        }

        private void InitializeThemes()
        {
            AvailableThemes.Clear();
            foreach (var theme in _themeManagerService.GetAvailableThemes())
            {
                AvailableThemes.Add(theme);
            }
            SelectedTheme = AvailableThemes[0];
        }
    }
}