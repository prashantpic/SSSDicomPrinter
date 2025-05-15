using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Repositories;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;
using TheSSS.DICOMViewer.Common.Interfaces;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Infrastructure.Persistence
{
    public class FileSystemViewStateRepository : IViewStateRepository
    {
        private readonly string _baseDataPath;
        private readonly ILoggerAdapter _logger;

        public FileSystemViewStateRepository(ILoggerAdapter logger)
        {
            _logger = logger;
            _baseDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TheSSS", "DICOMViewer", "ViewState");
            Directory.CreateDirectory(_baseDataPath);
        }

        private string GetFilePath(string viewKey)
        {
            var safeKey = Uri.EscapeDataString(viewKey).Replace("%", "_");
            return Path.Combine(_baseDataPath, $"{safeKey}.json");
        }

        public async Task SaveStateAsync(string viewKey, ViewState viewState)
        {
            try
            {
                var filePath = GetFilePath(viewKey);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(viewState, options);
                await File.WriteAllTextAsync(filePath, jsonString);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to save view state for key: {viewKey}");
            }
        }

        public async Task<ViewState> LoadStateAsync(string viewKey)
        {
            try
            {
                var filePath = GetFilePath(viewKey);
                if (!File.Exists(filePath)) return null;

                var jsonString = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<ViewState>(jsonString);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to load view state for key: {viewKey}");
                return null;
            }
        }

        public async Task ClearStateAsync(string viewKey)
        {
            try
            {
                var filePath = GetFilePath(viewKey);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to clear view state for key: {viewKey}");
            }
        }

        public async Task SaveApplicationSettingsAsync(ApplicationSettings settings)
        {
            try
            {
                var filePath = Path.Combine(_baseDataPath, "applicationSettings.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(settings, options);
                await File.WriteAllTextAsync(filePath, jsonString);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save application settings.");
            }
        }

        public async Task<ApplicationSettings> LoadApplicationSettingsAsync()
        {
            try
            {
                var filePath = Path.Combine(_baseDataPath, "applicationSettings.json");
                if (!File.Exists(filePath)) return null;

                var jsonString = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<ApplicationSettings>(jsonString);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load application settings.");
                return null;
            }
        }
    }
}