using SkiaSharp.Views.Desktop;
using System.Windows;
using System.Windows.Input;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Controls
{
    public partial class DicomImageView
    {
        private Point _lastMousePosition;
        private bool _isPanning;

        public DicomImageView()
        {
            InitializeComponent();
        }

        private void SKElement_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (DataContext is DicomImageViewModel viewModel)
            {
                viewModel.Render(e.Surface.Canvas, e.Info.Rect);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePosition = e.GetPosition(SKElement);
            _isPanning = true;
            SKElement.CaptureMouse();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isPanning || DataContext is not DicomImageViewModel viewModel) return;
            
            var currentPosition = e.GetPosition(SKElement);
            var delta = currentPosition - _lastMousePosition;
            viewModel.UpdatePanZoom(new Point(delta.X, delta.Y), 1);
            _lastMousePosition = currentPosition;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isPanning = false;
            SKElement.ReleaseMouseCapture();
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is not DicomImageViewModel viewModel) return;
            
            var zoomDelta = e.Delta > 0 ? 1.1 : 0.9;
            viewModel.UpdatePanZoom(new Point(0, 0), zoomDelta);
        }
    }
}