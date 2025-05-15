using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TheSSS.DicomViewer.Presentation.Services;
using TheSSS.DicomViewer.Presentation.ViewModels.Tabs;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _selectedTabViewModel;

        public ObservableCollection<object> TabViewModels { get; } = new();

        private readonly INavigationService _navigationService;

        public MainViewModel(
            INavigationService navigationService,
            IncomingPrintQueueTabViewModel incomingPrintQueueTabViewModel,
            LocalStorageTabViewModel localStorageTabViewModel,
            QueryRetrieveTabViewModel queryRetrieveTabViewModel)
        {
            _navigationService = navigationService;
            
            TabViewModels.Add(incomingPrintQueueTabViewModel);
            TabViewModels.Add(localStorageTabViewModel);
            TabViewModels.Add(queryRetrieveTabViewModel);
            
            SelectedTabViewModel = TabViewModels[0];
        }

        [RelayCommand]
        private void OpenSettings()
        {
            _navigationService.NavigateTo<SettingsShellViewModel>();
        }
    }
}