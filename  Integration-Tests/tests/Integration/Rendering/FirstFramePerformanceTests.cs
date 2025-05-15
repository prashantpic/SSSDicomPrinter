// Assuming DicomImageViewerViewModel is defined in TheSSS.DicomViewer.Presentation.ViewModels
// and UiInteractionHelper, PerformanceMetricsHelper, DicomTestDatasetManager in TheSSS.DicomViewer.IntegrationTests.Helpers/Fixtures
using Microsoft.Extensions.Configuration;
using TheSSS.DicomViewer.IntegrationTests.Fixtures;
using TheSSS.DicomViewer.IntegrationTests.Helpers;
using TheSSS.DicomViewer.Presentation.ViewModels; // For DicomImageViewerViewModel

namespace TheSSS.DicomViewer.IntegrationTests.Rendering;

[Trait("Category", "Performance")]
[Collection("PerformanceSensitiveTests")] // Ensures performance tests run with specific considerations (e.g., sequentially)
public class FirstFramePerformanceTests : IClassFixture<AppHostFixture>
{
    private readonly AppHostFixture _fixture;
    private readonly DicomTestDatasetManager _datasetManager;
    private readonly UiInteractionHelper _uiHelper;
    // PerformanceMetricsHelper is static, no need to inject via constructor
    private readonly TimeSpan _renderThreshold;
    private readonly DicomImageViewerViewModel _imageViewerViewModel;


    public FirstFramePerformanceTests(AppHostFixture fixture)
    {
        _fixture = fixture;
        _datasetManager = _fixture.ServiceProvider.GetRequiredService<DicomTestDatasetManager>();
        _uiHelper = _fixture.ServiceProvider.GetRequiredService<UiInteractionHelper>();
        
        var configuration = _fixture.ServiceProvider.GetRequiredService<IConfiguration>();
        var thresholdMs = configuration.GetValue<int>("PerformanceThresholds:FirstFrameRenderMs", 5000);
        _renderThreshold = TimeSpan.FromMilliseconds(thresholdMs);

        // Resolve the ViewModel responsible for loading/displaying the image
        // AppHostFixture must be configured to provide DicomImageViewerViewModel
        _imageViewerViewModel = _fixture.ServiceProvider.GetRequiredService<DicomImageViewerViewModel>();
        _imageViewerViewModel.Should().NotBeNull("DicomImageViewerViewModel should be resolvable from the fixture.");
    }

    private async Task PerformRenderTest(string datasetDisplayName, Func<string> getDatasetPathAction, Func<DicomTestDatasetManager, string, string?> getFilePathAction)
    {
        var datasetPathOrFile = getDatasetPathAction();
        string? dicomFilePath = getFilePathAction(_datasetManager, datasetPathOrFile);

        dicomFilePath.Should().NotBeNullOrEmpty($"Test file path for {datasetDisplayName} should exist.");

        // Ensure ViewModel is in a clean state before loading
        _imageViewerViewModel.ResetView(); // Assuming such a method exists

        var elapsed = await PerformanceMetricsHelper.MeasureExecutionTimeAsync(async () =>
        {
            await _uiHelper.TriggerImageLoadAsync(_imageViewerViewModel, dicomFilePath!);
            // If TriggerImageLoadAsync doesn't await rendering completion,
            // we might need to await a property/event on the ViewModel.
            // E.g., await _imageViewerViewModel.WaitForRenderCompletionAsync(TimeSpan.FromSeconds(10));
            // For simplicity, assume TriggerImageLoadAsync is sufficient for this integration test's purpose
            // or that the ViewModel's command execution includes initial rendering.
        });

        elapsed.Should().BeLessThanOrEqualTo(_renderThreshold, 
            $"Rendering the first frame of {datasetDisplayName} took {elapsed.TotalMilliseconds:F0}ms, exceeding threshold of {_renderThreshold.TotalMilliseconds:F0}ms.");
        
        _imageViewerViewModel.IsImageLoaded.Should().BeTrue($"Image from {datasetDisplayName} should be loaded after the operation.");
    }

    [Fact]
    [Trait("Requirement", "REQ-DID-005")]
    public async Task RenderFirstFrame_Large2GBMonochromeDataset_ShouldBeWithin5Seconds()
    {
        await PerformRenderTest(
            "Large 2GB Monochrome Dataset",
            () => _datasetManager.GetLargeDatasetForPerformanceTest(), // This returns a directory path
            (manager, path) => manager.GetAllFilePathsFromDataset(path).FirstOrDefault() // Get first .dcm file from directory
        );
    }

    [Fact]
    [Trait("Requirement", "REQ-DID-005")]
    public async Task RenderFirstFrame_StandardMonochromeCTSeries_ShouldBeWithin5Seconds()
    {
        await PerformRenderTest(
            "Standard Monochrome CT Series",
            () => _datasetManager.GetStandardMonochromeCTDatasetPath(), // This returns a directory path
            (manager, path) => manager.GetAllFilePathsFromDataset(path).FirstOrDefault()
        );
    }

    [Fact]
    [Trait("Requirement", "REQ-DID-005")]
    public async Task RenderFirstFrame_StandardColorUltrasoundSequence_ShouldBeWithin5Seconds()
    {
        await PerformRenderTest(
            "Standard Color Ultrasound Sequence",
            () => _datasetManager.GetStandardColorUltrasoundDatasetPath(), // This returns a directory path
            (manager, path) => manager.GetAllFilePathsFromDataset(path).FirstOrDefault()
        );
    }
}