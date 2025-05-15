using CommunityToolkit.Mvvm.ComponentModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels;

public partial class ThumbnailViewModel : ObservableObject
{
    [ObservableProperty]
    private ImageSource? _thumbnailImage;
    
    [ObservableProperty]
    private int _instanceNumber;
    
    [ObservableProperty]
    private bool _isSelected;
}