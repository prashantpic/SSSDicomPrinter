using CommunityToolkit.Mvvm.ComponentModel;

namespace TheSSS.DICOMViewer.Presentation.Rendering
{
    public partial class RenderingOptions : ObservableObject
    {
        [ObservableProperty]
        private double windowCenter = 127.5;
        
        [ObservableProperty]
        private double windowWidth = 255;
        
        [ObservableProperty]
        private double zoomFactor = 1.0;
        
        [ObservableProperty]
        private double panOffsetX;
        
        [ObservableProperty]
        private double panOffsetY;
        
        [ObservableProperty]
        private double rotationAngle;
        
        [ObservableProperty]
        private bool flipHorizontal;
        
        [ObservableProperty]
        private bool flipVertical;
    }
}