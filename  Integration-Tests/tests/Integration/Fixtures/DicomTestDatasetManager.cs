using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TheSSS.DicomViewer.IntegrationTests.Fixtures
{
    public class DicomTestDatasetManager
    {
        private readonly string _testDataRootPath;
        private readonly IConfiguration _configuration;

        public DicomTestDatasetManager(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _testDataRootPath = _configuration["TestDataPath"] ?? "TestData"; // Default to "TestData" relative to execution

            // Ensure the base path is absolute for reliability
            if (!Path.IsPathRooted(_testDataRootPath))
            {
                _testDataRootPath = Path.Combine(AppContext.BaseDirectory, _testDataRootPath);
            }

            if (!Directory.Exists(_testDataRootPath))
            {
                // Attempt to create if it doesn't exist, useful for some CI scenarios
                // Directory.CreateDirectory(_testDataRootPath);
                // For this exercise, we assume it exists or tests requiring it will fail meaningfully.
                Console.WriteLine($"Warning: Test data root path '{_testDataRootPath}' not found.");
            }
        }

        public string GetDatasetPath(string datasetName)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be null or whitespace.", nameof(datasetName));
            }
            return Path.Combine(_testDataRootPath, datasetName);
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
            return Path.Combine(GetDatasetPath(datasetName), fileName);
        }

        public string GetLargeDatasetForPerformanceTest()
        {
            // As per SDS: Path.Combine(TestDataPath, "rendering_performance", "dicom_2gb_dataset_1")
            // This can be made configurable via appsettings.IntegrationTests.json if needed.
            // For now, hardcoding the subpath relative to TestDataRootPath.
            string largeDatasetRelativePath = _configuration["TestDataPaths:RenderingPerformanceLargeDataset"]
                                              ?? Path.Combine("rendering_performance", "dicom_2gb_dataset_1");
            return Path.Combine(_testDataRootPath, largeDatasetRelativePath);
        }

        public IEnumerable<string> GetAllFilePathsFromDataset(string datasetName, string searchPattern = "*.dcm")
        {
            var datasetPath = GetDatasetPath(datasetName);
            if (!Directory.Exists(datasetPath))
            {
                // Consider throwing FileNotFoundException or DirectoryNotFoundException
                // For now, returning empty or logging a warning is also an option.
                Console.WriteLine($"Warning: Dataset directory '{datasetPath}' not found for dataset '{datasetName}'.");
                return Enumerable.Empty<string>();
            }
            return Directory.EnumerateFiles(datasetPath, searchPattern, SearchOption.AllDirectories);
        }
    }
}