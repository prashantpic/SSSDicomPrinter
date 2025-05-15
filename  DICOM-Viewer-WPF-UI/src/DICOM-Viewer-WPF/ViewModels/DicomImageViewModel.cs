using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using TheSSS.DicomViewer.Presentation.Rendering;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class DicomImageViewModel : ObservableObject
    {
        private readonly IRenderer<SKCanvas, DicomImageViewModel, SKRect> _renderer;

        [ObservableProperty]
        private double _windowWidth = 400;

        [ObservableProperty]
        private double _windowLevel = 50;

        [ObservableProperty]
        private double _zoom = 1.0;

        [ObservableProperty]
        private Point _panOffset = new(0, 0);

        public DicomImageViewModel(IRenderer<SKCanvas, DicomImageViewModel, SKRect> renderer)
        {
            _renderer = renderer;
        }

        public void Render(SKCanvas canvas, SKRect destinationRect)
        {
            _renderer.Render(canvas, this, destinationRect);
        }

        public void UpdatePanZoom(Point panDelta, double zoomDelta)
        {
            PanOffset = new Point(PanOffset.X + panDelta.X, PanOffset.Y + panDelta.Y);
            Zoom *= zoomDelta;
        }
    }
}