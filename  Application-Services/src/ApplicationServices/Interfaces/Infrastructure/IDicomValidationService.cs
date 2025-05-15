using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Infrastructure
{
    public interface IDicomValidationService
    {
        Task<DicomValidationResult> ValidateDicomFileAsync(string filePath, CancellationToken cancellationToken);
    }
}