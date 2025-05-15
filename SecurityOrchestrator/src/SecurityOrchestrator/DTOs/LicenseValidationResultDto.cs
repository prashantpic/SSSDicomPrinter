using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Security.DTOs;

/// <summary>
/// Carries license validation outcome (IsValid, StatusMessage, ExpiryDate, Features).
/// Requirement REQ-LDM-LIC-002, REQ-LDM-LIC-004, REQ-LDM-LIC-005.
/// </summary>
public record LicenseValidationResultDto(
    bool IsValid,
    string StatusMessage,
    DateTime? ExpiryDate,
    IEnumerable<string>? Features
);