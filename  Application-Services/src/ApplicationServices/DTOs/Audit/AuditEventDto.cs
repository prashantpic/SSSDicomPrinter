using System;

namespace TheSSS.DicomViewer.Application.DTOs.Audit
{
    public record AuditEventDto(
        DateTimeOffset Timestamp,
        string EventType,
        string UserId,
        string Description,
        string StudyInstanceUid,
        string SopInstanceUid,
        string DetailsJson);
}