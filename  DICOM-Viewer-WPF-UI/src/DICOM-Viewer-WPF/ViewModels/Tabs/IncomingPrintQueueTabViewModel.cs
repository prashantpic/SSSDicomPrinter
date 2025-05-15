using CommunityToolkit.Mvvm.ComponentModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs
{
    public partial class IncomingPrintQueueTabViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _statusMessage = "Incoming Print Queue";
    }
}