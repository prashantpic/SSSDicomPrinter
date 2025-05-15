using SkiaSharp.Views.WPF;
using System.Windows;
using System.Windows.Controls;

namespace TheSSS.DICOMViewer.Presentation.Views.Controls
{
    public partial class DicomImageView : UserControl
    {
        public DicomImageView()
        {
            InitializeComponent();
        }

        private void SKElement_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (DataContext is ViewModels.DicomImageViewModel viewModel)
            {
                viewModel.OnPaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
        }
    }
}