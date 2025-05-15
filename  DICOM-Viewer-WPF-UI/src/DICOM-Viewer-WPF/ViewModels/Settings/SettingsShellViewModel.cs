using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Settings
{
    public partial class SettingsShellViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? _currentPanelViewModel;

        [RelayCommand]
        private void NavigateToPanel(string panelType)
        {
        }
    }
}