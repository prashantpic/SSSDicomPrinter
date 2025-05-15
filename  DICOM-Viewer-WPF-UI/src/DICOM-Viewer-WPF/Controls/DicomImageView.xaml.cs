using SkiaSharp.Views.WPF;
using System.Windows;
using System.Windows.Input;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Controls
{
    public partial class DicomImageView
    {
        private Point _lastMousePosition;

        public DicomImageView()
        {
            InitializeComponent();
        }

        private void SKElement_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            if (DataContext is DicomImageViewModel viewModel)
            {
                viewModel.Render(e.Surface.Canvas, e.Info.Rect);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePosition = e.GetPosition(this);
            CaptureMouse();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && DataContext is DicomImageViewModel viewModel)
            {
                var currentPosition = e.GetPosition(this);
                var delta = new Point(currentPosition.X - _lastMousePosition.X, currentPosition.Y - _lastMousePosition.Y);
                viewModel.UpdatePanZoom(delta, 1.0);
                _lastMousePosition = currentPosition;
                skElement.InvalidateVisual();
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is DicomImageViewModel viewModel)
            {
                viewModel.UpdatePanZoom(new Point(0, 0), e.Delta > 0 ? 1.1 : 0.9);
                skElement.InvalidateVisual();
            }
        }
    }
}