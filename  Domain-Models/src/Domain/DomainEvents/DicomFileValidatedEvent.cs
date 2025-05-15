using System;
using TheSSS.DicomViewer.Domain.Validation;

namespace TheSSS.DicomViewer.Domain.DomainEvents
{
    public class DicomFileValidatedEvent : IDomainEvent
    {
        public string FilePath { get; }
        public ComplianceReport Report { get; }
        public DateTimeOffset OccurredOn { get; }

        public DicomFileValidatedEvent(string path, ComplianceReport report)
        {
            FilePath = path ?? throw new ArgumentNullException(nameof(path));
            Report = report ?? throw new ArgumentNullException(nameof(report));
            OccurredOn = DateTimeOffset.UtcNow;
        }
    }
}