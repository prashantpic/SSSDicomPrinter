using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TheSSS.DicomViewer.IntegrationTests.Fixtures
{
    public class DicomTestDatasetManager
    {
        private readonly string _baseTestDataPath;

        public DicomTestDatasetManager(IConfiguration configuration)
        {
            _baseTestDataPath = configuration["TestDataPaths:DicomRoot"] 
                ?? throw new InvalidOperationException("TestDataPaths:DicomRoot configuration not found in appsettings.IntegrationTests.json.");

            if (!Path.IsPathRooted(_baseTestDataPath))
            {
                _baseTestDataPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _baseTestDataPath));
            }

            if (!Directory.Exists(_baseTestDataPath))
            {
                throw new DirectoryNotFoundException($"DICOM test data root directory not found: {_baseTestDataPath}. Ensure TestData directory exists and appsettings.IntegrationTests.json points to it.");
            }
        }

        public string GetDatasetPath(string datasetName)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be null or whitespace.", nameof(datasetName));
            }

            var fullPath = Path.Combine(_baseTestDataPath, datasetName);
            if (!Directory.Exists(fullPath))
            {
                throw new DirectoryNotFoundException($"Test dataset directory '{datasetName}' not found at expected path: {fullPath}");
            }
            return fullPath;
        }

        public string GetFilePath(string datasetName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be null or whitespace.", nameof(datasetName));
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));
            }

            var datasetPath = GetDatasetPath(datasetName); // This will throw if datasetName is invalid
            var filePath = Path.Combine(datasetPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Test data file '{fileName}' not found in dataset '{datasetName}' at expected path: {filePath}");
            }
            return filePath;
        }

        public string GetLargeDatasetForPerformanceTest()
        {
            // As per spec: REQ-DID-005 (e.g., 2GB)
            // The actual dataset name should match what's in the TestData folder structure
            // Example name from spec's file structure: "rendering_performance/dicom_2gb_dataset_1"
            return GetDatasetPath(Path.Combine("rendering_performance", "dicom_2gb_dataset_1"));
        }

        public IEnumerable<string> GetAllFilePathsFromDataset(string datasetName)
        {
            var datasetPath = GetDatasetPath(datasetName);
            // Assuming DICOM files have .dcm extension, search recursively.
            return Directory.EnumerateFiles(datasetPath, "*.dcm", SearchOption.AllDirectories);
        }
    }
}