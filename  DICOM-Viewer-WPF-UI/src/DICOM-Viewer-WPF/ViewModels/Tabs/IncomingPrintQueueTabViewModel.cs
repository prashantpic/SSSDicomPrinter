using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DicomViewer.Presentation.Services.Application;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs
{
    public partial class IncomingPrintQueueTabViewModel : ObservableObject
    {
        private readonly IPrintOrchestrationService _printService;

        public IncomingPrintQueueTabViewModel(IPrintOrchestrationService printService)
        {
            _printService = printService;
        }

        [RelayCommand]
        private async Task RefreshQueueAsync()
        {
            await _printService.RefreshPrintQueueAsync();
        }
    }
}