using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class ThumbnailGridViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ThumbnailViewModel> _thumbnails = new();

        [ObservableProperty]
        private ThumbnailViewModel? _selectedThumbnail;
    }
}