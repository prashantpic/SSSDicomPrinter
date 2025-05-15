using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Infrastructure
{
    public interface IFileSystemService
    {
        Task<IEnumerable<string>> EnumerateFilesAsync(string path, CancellationToken cancellationToken = default);
        Task<Stream> ReadFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task WriteFileAsync(string filePath, Stream content, CancellationToken cancellationToken = default);
        Task MoveFileAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
        Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);
    }
}