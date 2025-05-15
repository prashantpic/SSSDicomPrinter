using System;

namespace TheSSS.DICOMViewer.Security.DTOs;

/// <summary>
/// Carries details for audit logging (EventType, UserId, Timestamp, Outcome, Details, SourceIP).
/// Requirement REQ-7-001, REQ-7-027.
/// </summary>
public record SecurityEventDetailsDto(
    string EventType,
    DateTime Timestamp,
    string Outcome,
    string Details,
    string? UserId = null,
    string? SourceIP = null
);