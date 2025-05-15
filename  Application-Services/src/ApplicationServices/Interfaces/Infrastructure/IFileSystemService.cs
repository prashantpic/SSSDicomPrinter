using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Infrastructure
{
    public interface IFileSystemService
    {
        Task CreateDirectoryAsync(string path, CancellationToken cancellationToken);
        Task SaveFileAsync(string filePath, Stream content, CancellationToken cancellationToken);
        Task MoveFileAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken);
        Task DeleteFileAsync(string filePath, CancellationToken cancellationToken);
        Task<List<string>> ListFilesInDirectoryAsync(string directoryPath, CancellationToken cancellationToken);
        Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken);
    }
}