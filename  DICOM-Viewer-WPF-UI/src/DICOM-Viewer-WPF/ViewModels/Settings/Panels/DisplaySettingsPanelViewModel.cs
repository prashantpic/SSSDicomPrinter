using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DicomViewer.Presentation.Services;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Settings.Panels;

public partial class DisplaySettingsPanelViewModel : ObservableObject
{
    private readonly IThemeManager _themeManager;
    
    [ObservableProperty]
    private ObservableCollection<string> _availableThemes = new() { "Light", "Dark", "High Contrast" };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentThemeDisplay))]
    private string _selectedTheme;

    [ObservableProperty]
    private bool _isHighContrastEnabled;

    public string CurrentThemeDisplay => $"Current Theme: {SelectedTheme}";

    public DisplaySettingsPanelViewModel(IThemeManager themeManager)
    {
        _themeManager = themeManager;
        LoadCurrentSettings();
    }

    [RelayCommand]
    private void SaveSettings()
    {
        _themeManager.SetTheme(SelectedTheme);
        // Placeholder for actual settings persistence
    }

    private void LoadCurrentSettings()
    {
        SelectedTheme = _themeManager.GetCurrentTheme();
        // Placeholder for loading other display settings
    }
}