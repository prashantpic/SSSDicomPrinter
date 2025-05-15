using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class DicomImageViewModel : ObservableObject
    {
        [ObservableProperty]
        private byte[]? _pixelData;

        [ObservableProperty]
        private int _imageWidth;

        [ObservableProperty]
        private int _imageHeight;

        [ObservableProperty]
        private string _photometricInterpretation = string.Empty;

        [ObservableProperty]
        private double _windowWidth = 400;

        [ObservableProperty]
        private double _windowLevel = 50;

        public async Task LoadImageAsync(string filePath)
        {
            await Task.CompletedTask; // Implement DICOM loading
        }

        public void ApplyWindowLevel(double newWidth, double newLevel)
        {
            WindowWidth = newWidth;
            WindowLevel = newLevel;
        }
    }
}