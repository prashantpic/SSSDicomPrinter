using CommunityToolkit.Mvvm.ComponentModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class ThumbnailViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _instanceUid;

        [ObservableProperty]
        private int _instanceNumber;
    }
}