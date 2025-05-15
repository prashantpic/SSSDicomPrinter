using System.IO;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

public interface IDicomFileStoreAdapter
{
    Task<string> StoreDicomFileAsync(string sourceFilePath, string patientId, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid);
    Task<Stream> GetDicomFileAsync(string sopInstanceUid);
    Task<string> GetDicomFilePathAsync(string sopInstanceUid);
    Task MoveToQuarantineAsync(string filePath, string reason);
}