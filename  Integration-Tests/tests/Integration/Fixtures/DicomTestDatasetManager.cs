using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace TheSSS.DicomViewer.IntegrationTests.Fixtures;

public class DicomTestDatasetManager
{
    private readonly string _baseTestDataPath;

    public DicomTestDatasetManager(IConfiguration configuration)
    {
        _baseTestDataPath = configuration["TestDataPaths:DicomRoot"]
            ?? throw new InvalidOperationException("Configuration key 'TestDataPaths:DicomRoot' not found in appsettings.IntegrationTests.json.");

        if (!Path.IsPathRooted(_baseTestDataPath))
        {
            // Assuming tests run from a directory where AppContext.BaseDirectory is appropriate (e.g., bin/Debug/net8.0)
            _baseTestDataPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _baseTestDataPath));
        }

        if (!Directory.Exists(_baseTestDataPath))
        {
            throw new DirectoryNotFoundException($"The configured DICOM test data root directory was not found: {_baseTestDataPath}. " +
                $"Ensure 'TestData' directory exists relative to the test execution path or an absolute path is configured correctly.");
        }
    }

    public string GetDatasetPath(string datasetName)
    {
        var fullPath = Path.Combine(_baseTestDataPath, datasetName);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"Test dataset directory '{datasetName}' not found at expected path: {fullPath}");
        }
        return fullPath;
    }

    public string GetFilePath(string datasetName, string fileName)
    {
        var datasetFullPath = GetDatasetPath(datasetName); // This will throw if datasetName is invalid
        var filePath = Path.Combine(datasetFullPath, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test data file '{fileName}' not found in dataset '{datasetName}' at expected path: {filePath}");
        }
        return filePath;
    }

    public string GetLargeDatasetForPerformanceTest()
    {
        // This name must match the directory structure in TestData
        return GetDatasetPath("rendering_performance/dicom_2gb_dataset_1");
    }

    public IEnumerable<string> GetAllFilePathsFromDataset(string datasetName)
    {
        var datasetPath = GetDatasetPath(datasetName);
        return Directory.EnumerateFiles(datasetPath, "*.dcm", SearchOption.AllDirectories);
    }

    // Convenience methods for specific, known test datasets
    public string GetStandardMonochromeCTDatasetPath()
    {
        return GetDatasetPath("rendering_performance/monochrome_ct_dataset");
    }

    public string GetStandardColorUltrasoundDatasetPath()
    {
        return GetDatasetPath("rendering_performance/color_us_dataset");
    }

    public string GetLicensingTestDataPath()
    {
        return GetDatasetPath("licensing");
    }
    
    public string GetValidLicenseKeyPath()
    {
        return GetFilePath("licensing", "valid_key.txt");
    }

    public string GetInvalidLicenseKeyPath()
    {
        return GetFilePath("licensing", "invalid_key.txt");
    }
    
    public string GetSearchPerformanceDataPath()
    {
        return GetDatasetPath("search_performance");
    }

    public string GetMetadataSeedTemplatePath()
    {
        return GetFilePath("search_performance", "metadata_seed_template.dcm");
    }
}