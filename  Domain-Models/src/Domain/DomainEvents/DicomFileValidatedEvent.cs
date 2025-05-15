using System;
using TheSSS.DicomViewer.Domain.Validation;

namespace TheSSS.DicomViewer.Domain.DomainEvents
{
    public class DicomFileValidatedEvent : IDomainEvent
    {
        public string FilePath { get; }
        public ComplianceReport Report { get; }
        public DateTimeOffset OccurredOn { get; }

        public DicomFileValidatedEvent(string filePath, ComplianceReport report, DateTimeOffset occurredOn)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Report = report ?? throw new ArgumentNullException(nameof(report));
            OccurredOn = occurredOn;
        }
    }
}