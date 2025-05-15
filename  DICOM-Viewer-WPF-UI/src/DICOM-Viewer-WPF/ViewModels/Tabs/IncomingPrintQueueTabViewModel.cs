using CommunityToolkit.Mvvm.ComponentModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs;

public partial class IncomingPrintQueueTabViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    public IncomingPrintQueueTabViewModel()
    {
        // Initialize with sample data or service injection
    }
}