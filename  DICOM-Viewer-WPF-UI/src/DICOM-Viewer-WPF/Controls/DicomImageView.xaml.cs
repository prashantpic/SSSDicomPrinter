using SkiaSharp.Views.Desktop;
using System.Windows;
using System.Windows.Input;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Controls;

public partial class DicomImageView
{
    private Point _lastMousePosition;
    private bool _isDragging;

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
        _lastMousePosition = e.GetPosition(this);
        _isDragging = true;
        CaptureMouse();
    }

    private void Image_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && DataContext is DicomImageViewModel viewModel)
        {
            var currentPosition = e.GetPosition(this);
            var delta = currentPosition - _lastMousePosition;
            viewModel.PanOffset = new Point(
                viewModel.PanOffset.X + delta.X,
                viewModel.PanOffset.Y + delta.Y
            );
            _lastMousePosition = currentPosition;
            SKElement.InvalidateVisual();
        }
    }

    private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        ReleaseMouseCapture();
    }

    private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is DicomImageViewModel viewModel)
        {
            viewModel.Zoom += e.Delta * 0.001;
            SKElement.InvalidateVisual();
        }
    }
}