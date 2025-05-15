using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DicomViewer.Presentation.Services;
using TheSSS.DicomViewer.Presentation.ViewModels.Tabs;

namespace TheSSS.DicomViewer.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    
    [ObservableProperty]
    private object? _selectedTabViewModel;

    public MainViewModel(
        INavigationService navigationService,
        IncomingPrintQueueTabViewModel incomingPrintQueueTabViewModel,
        LocalStorageTabViewModel localStorageTabViewModel,
        QueryRetrieveTabViewModel queryRetrieveTabViewModel)
    {
        _navigationService = navigationService;
        TabViewModels = new object[]
        {
            incomingPrintQueueTabViewModel,
            localStorageTabViewModel,
            queryRetrieveTabViewModel
        };
        SelectedTabViewModel = TabViewModels.FirstOrDefault();
    }

    public object[] TabViewModels { get; }
    
    [RelayCommand]
    private void OpenSettings()
    {
        _navigationService.NavigateTo<SettingsShellViewModel>();
    }
}