using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities.Anonymization
{
    public class LogAnonymizationActivity : IWorkflowActivity<SagaStateBase>
    {
        private readonly IAuditLoggerAdapter _auditLogger;

        public LogAnonymizationActivity(IAuditLoggerAdapter auditLogger)
        {
            _auditLogger = auditLogger;
        }

        public async Task<bool> ExecuteAsync(SagaStateBase state)
        {
            var details = new
            {
                WorkflowId = state.WorkflowId,
                Operation = "DICOM Anonymization",
                Timestamp = DateTime.UtcNow,
                Metadata = "PHI redaction completed"
            };

            await _auditLogger.LogAuditEntryAsync("Anonymization", details.ToString());
            return true;
        }
    }
}