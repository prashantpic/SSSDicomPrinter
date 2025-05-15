using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class ThumbnailGridViewModel : ObservableObject
    {
        public ObservableCollection<ThumbnailViewModel> Thumbnails { get; } = new();

        [ObservableProperty]
        private ThumbnailViewModel _selectedThumbnail;
    }
}