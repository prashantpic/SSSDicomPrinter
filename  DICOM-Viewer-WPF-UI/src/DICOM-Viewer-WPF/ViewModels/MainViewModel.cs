using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DicomViewer.Presentation.Services;
using System.Collections.ObjectModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private object? _selectedTabViewModel;

        public ObservableCollection<object> TabViewModels { get; } = new();

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
            SelectedTabViewModel = TabViewModels.First();
        }

        [RelayCommand]
        private void OpenSettings()
        {
            _navigationService.NavigateTo<SettingsShellViewModel>();
        }
    }
}