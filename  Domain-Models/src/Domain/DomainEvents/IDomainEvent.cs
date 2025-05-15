using System;

namespace TheSSS.DicomViewer.Domain.DomainEvents
{
    public interface IDomainEvent
    {
        DateTimeOffset OccurredOn { get; }
    }
}