using System;

namespace TheSSS.DicomViewer.Application.DTOs.Audit
{
    public record AuditEventDto(
        DateTime Timestamp,
        string UserIdentifier,
        string EventType,
        string Description,
        string Source,
        string EntityType,
        string EntityIdentifier,
        string DetailsJson);
}