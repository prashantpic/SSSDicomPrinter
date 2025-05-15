using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SkiaSharp.Views.Desktop;

namespace TheSSS.DicomViewer.Presentation.Controls
{
    public partial class DicomImageView : UserControl
    {
        public DicomImageView()
        {
            InitializeComponent();
        }

        private void SkElement_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Black);
        }

        private void SkElement_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Handle zoom logic
        }

        private void SkElement_MouseMove(object sender, MouseEventArgs e)
        {
            // Handle pan logic
        }

        private void SkElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Handle interaction start
        }
    }
}