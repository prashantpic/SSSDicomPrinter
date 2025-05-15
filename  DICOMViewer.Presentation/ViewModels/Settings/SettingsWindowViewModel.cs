using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TheSSS.DICOMViewer.Presentation.ViewModels.Settings
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject? _selectedPanelViewModel;

        [ObservableProperty]
        private ObservableCollection<ObservableObject> _panelViewModels = new();

        public SettingsWindowViewModel(
            ThemeSettingsPanelViewModel themeSettingsPanelVM,
            LocalizationSettingsPanelViewModel localizationSettingsPanelVM)
        {
            PanelViewModels.Add(themeSettingsPanelVM);
            PanelViewModels.Add(localizationSettingsPanelVM);
            SelectedPanelViewModel = PanelViewModels[0];
        }

        [RelayCommand]
        private void ApplySettings()
        {
            // Implementation for applying settings
        }

        [RelayCommand]
        private void CancelSettings()
        {
            // Implementation for canceling changes
        }
    }
}