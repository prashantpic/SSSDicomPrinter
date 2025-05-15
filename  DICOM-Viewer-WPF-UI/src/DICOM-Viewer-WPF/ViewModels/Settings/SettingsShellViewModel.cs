using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Settings;

public partial class SettingsShellViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentPanelViewModel;
    
    [ObservableProperty]
    private ObservableCollection<object> _panelNavigationItems = new();
}