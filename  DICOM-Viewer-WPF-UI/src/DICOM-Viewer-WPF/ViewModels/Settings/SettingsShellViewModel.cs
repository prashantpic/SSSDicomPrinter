using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Settings
{
    public partial class SettingsShellViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<object> _panelNavigationItems = new();

        [ObservableProperty]
        private object? _currentPanelViewModel;

        public SettingsShellViewModel()
        {
            InitializeNavigationItems();
        }

        private void InitializeNavigationItems()
        {
            // TODO: Implement navigation items
        }
    }
}