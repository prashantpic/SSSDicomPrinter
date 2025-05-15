using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class ThumbnailViewModel : ObservableObject
    {
        [ObservableProperty]
        private ImageSource? _thumbnailImage;

        [ObservableProperty]
        private int _instanceNumber;
    }
}