using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Auditing
{
    public class AuditEntry
    {
        public AuditLogId Id { get; init; }
        public DateTime Timestamp { get; init; }
        public string EventType { get; init; }
        public string EventDetails { get; init; }

        public AuditEntry(AuditLogId id, DateTime timestamp, string eventType, string details)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Timestamp = timestamp;
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            EventDetails = details ?? throw new ArgumentNullException(nameof(details));
        }
    }
}