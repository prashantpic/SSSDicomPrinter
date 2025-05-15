using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DicomViewer.Presentation.Services.Application;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs
{
    public partial class LocalStorageTabViewModel : ObservableObject
    {
        private readonly IDicomSearchService _dicomSearchService;

        public LocalStorageTabViewModel(IDicomSearchService dicomSearchService)
        {
            _dicomSearchService = dicomSearchService;
        }

        [RelayCommand]
        private async Task SearchStudiesAsync()
        {
            await _dicomSearchService.SearchLocalStudiesAsync();
        }
    }
}