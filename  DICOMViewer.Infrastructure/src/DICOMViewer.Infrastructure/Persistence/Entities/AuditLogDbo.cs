namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class AuditLogDbo
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserIdentifier { get; set; }
        public string EventType { get; set; }
        public string EventDetails { get; set; }
        public string Outcome { get; set; }
        public string SourceIp { get; set; }
    }
}