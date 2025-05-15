using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Auditing
{
    public class AuditEntry
    {
        public AuditLogId Id { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string EventType { get; private set; }
        public string EventDetails { get; private set; }

        private AuditEntry() { }

        public AuditEntry(AuditLogId id, DateTime timestamp, string eventType, string eventDetails)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Timestamp = timestamp;
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            EventDetails = eventDetails ?? throw new ArgumentNullException(nameof(eventDetails));
        }
    }
}