namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class AuditLogDbo
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string UserIdentifier { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Details { get; set; }
        public string Outcome { get; set; }
        public string IpAddress { get; set; }
    }
}