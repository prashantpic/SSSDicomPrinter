using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Application
{
    public interface IDicomImportService
    {
        Task<DicomImportResultDto> ImportFromPathAsync(string path, CancellationToken cancellationToken);
        Task<DicomImportResultDto> ImportFromHoldingFolderAsync(string studyInstanceUid, CancellationToken cancellationToken);
    }
}