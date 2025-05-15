using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Repositories;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Infrastructure.Persistence
{
    public class FileSystemViewStateRepository : IViewStateRepository
    {
        private readonly string _storagePath;

        public FileSystemViewStateRepository()
        {
            _storagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DICOMViewer",
                "ViewStates");
            
            Directory.CreateDirectory(_storagePath);
        }

        public async Task SaveStateAsync(string viewKey, ViewState viewState)
        {
            var filePath = Path.Combine(_storagePath, $"{viewKey}.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            
            await using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, viewState, options);
        }

        public async Task<ViewState?> LoadStateAsync(string viewKey)
        {
            var filePath = Path.Combine(_storagePath, $"{viewKey}.json");
            
            if (!File.Exists(filePath))
                return null;

            await using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<ViewState>(stream);
        }
    }
}