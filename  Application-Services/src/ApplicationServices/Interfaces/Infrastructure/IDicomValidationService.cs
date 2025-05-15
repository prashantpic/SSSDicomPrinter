using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Infrastructure
{
    public interface IDicomValidationService
    {
        Task<bool> ValidateDicomFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task<DicomValidationResult> ValidateComplianceAsync(string filePath, CancellationToken cancellationToken = default);
    }

    public class DicomValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}