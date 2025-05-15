using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DICOMViewer.Presentation.Models;
using TheSSS.DICOMViewer.Presentation.Rendering;

namespace TheSSS.DICOMViewer.Presentation.ViewModels
{
    public partial class DicomImageViewModel : ObservableObject
    {
        private readonly IDicomDataProviderService _dicomDataProviderService;
        private readonly ISkiaDicomDrawer _skiaDicomDrawer;

        [ObservableProperty]
        private DicomFrameData? _currentFrameData;

        [ObservableProperty]
        private int _currentFrameIndex;

        [ObservableProperty]
        private int _totalFrames;

        [ObservableProperty]
        private RenderingOptions _renderingOptions = new();

        public DicomImageViewModel(
            IDicomDataProviderService dicomDataProviderService,
            ISkiaDicomDrawer skiaDicomDrawer)
        {
            _dicomDataProviderService = dicomDataProviderService;
            _skiaDicomDrawer = skiaDicomDrawer;
        }

        [RelayCommand]
        private async Task LoadImageAsync(string seriesInstanceUid)
        {
            var seriesData = await _dicomDataProviderService.GetSeriesDataAsync(seriesInstanceUid);
            TotalFrames = seriesData.FrameCount;
            CurrentFrameIndex = 0;
        }

        public void OnPaintSurface(SKCanvas canvas, int width, int height)
        {
            if (CurrentFrameData != null)
            {
                _skiaDicomDrawer.DrawFrame(canvas, CurrentFrameData, RenderingOptions, width, height);
            }
        }
    }
}