using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DicomViewer.Presentation.Services.Application;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs
{
    public partial class QueryRetrieveTabViewModel : ObservableObject
    {
        private readonly IDicomNetworkService _dicomNetworkService;

        public QueryRetrieveTabViewModel(IDicomNetworkService dicomNetworkService)
        {
            _dicomNetworkService = dicomNetworkService;
        }

        [RelayCommand]
        private async Task QueryPacsAsync()
        {
            await _dicomNetworkService.QueryRemotePacsAsync();
        }
    }
}