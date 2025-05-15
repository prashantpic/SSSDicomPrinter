using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TheSSS.DicomViewer.Presentation.Rendering;

namespace TheSSS.DicomViewer.Presentation.ViewModels;

public partial class DicomImageViewModel : ObservableObject
{
    private readonly IRenderer _renderer;
    
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
    
    [ObservableProperty]
    private double _zoom = 1.0;
    
    [ObservableProperty]
    private Point _panOffset;

    public DicomImageViewModel(IRenderer renderer)
    {
        _renderer = renderer;
    }

    public async Task LoadImageAsync(string filePath)
    {
        // Implementation would call application layer service
    }

    [RelayCommand]
    public void ApplyWindowLevel(double newWidth, double newLevel)
    {
        WindowWidth = newWidth;
        WindowLevel = newLevel;
    }

    public void Render(SKCanvas canvas, SKRect destinationRect)
    {
        _renderer.Render(canvas, this, destinationRect);
    }
}