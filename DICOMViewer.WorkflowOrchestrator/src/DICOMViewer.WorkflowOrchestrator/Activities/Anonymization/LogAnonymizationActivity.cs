using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities.Anonymization
{
    public class LogAnonymizationActivity
    {
        private readonly IAuditLoggerAdapter _auditLogger;

        public LogAnonymizationActivity(IAuditLoggerAdapter auditLogger)
        {
            _auditLogger = auditLogger;
        }

        public async Task ExecuteAsync(Guid workflowId, Guid studyId, Guid profileId, CancellationToken cancellationToken)
        {
            await _auditLogger.LogAuditEventAsync(
                "AnonymizationCompleted",
                $"Study {studyId} anonymized with profile {profileId}",
                null,
                studyId,
                null,
                workflowId,
                new { ProfileId = profileId, Timestamp = DateTime.UtcNow }
            );
        }
    }
}