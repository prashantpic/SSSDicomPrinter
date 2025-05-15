using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class ThumbnailGridViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ThumbnailViewModel> _thumbnails = new();

        [ObservableProperty]
        private ThumbnailViewModel? _selectedThumbnail;

        [RelayCommand]
        private void SelectThumbnail(ThumbnailViewModel? thumbnail)
        {
            SelectedThumbnail = thumbnail;
        }

        public ThumbnailGridViewModel()
        {
            Thumbnails.Add(new ThumbnailViewModel { InstanceNumber = 1 });
            Thumbnails.Add(new ThumbnailViewModel { InstanceNumber = 2 });
        }
    }
}