using System.Threading;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Application.DTOs.DicomImport;

namespace TheSSS.DicomViewer.Application.Interfaces.Application
{
    public interface IDicomImportService
    {
        Task<DicomImportResultDto> ImportFromPathAsync(string path, CancellationToken cancellationToken = default);
        Task<DicomImportResultDto> ImportFromHoldingFolderAsync(string studyInstanceUid, CancellationToken cancellationToken = default);
    }
}