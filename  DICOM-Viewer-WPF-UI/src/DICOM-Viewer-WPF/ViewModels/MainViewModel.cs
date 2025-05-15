using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TheSSS.DicomViewer.Presentation.Services;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<object> _tabViewModels = new();

        [ObservableProperty]
        private object? _selectedTabViewModel;

        public MainViewModel(INavigationService navigationService,
            IncomingPrintQueueTabViewModel incomingPrintQueueTabViewModel,
            LocalStorageTabViewModel localStorageTabViewModel,
            QueryRetrieveTabViewModel queryRetrieveTabViewModel)
        {
            _navigationService = navigationService;
            
            TabViewModels.Add(incomingPrintQueueTabViewModel);
            TabViewModels.Add(localStorageTabViewModel);
            TabViewModels.Add(queryRetrieveTabViewModel);
            
            SelectedTabViewModel = TabViewModels.FirstOrDefault();
        }

        [RelayCommand]
        private void OpenSettings()
        {
            _navigationService.NavigateTo<SettingsShellViewModel>();
        }
    }
}