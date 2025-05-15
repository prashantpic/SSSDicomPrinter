using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces
{
    public interface IAuditLoggerAdapter
    {
        Task LogAuditEntryAsync(string entryType, string details);
    }
}