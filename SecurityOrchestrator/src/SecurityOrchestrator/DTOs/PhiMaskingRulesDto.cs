using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Security.DTOs;

/// <summary>
/// Carries a dictionary of PHI masking rules.
/// Requirement REQ-7-004.
/// </summary>
public record PhiMaskingRulesDto(
    IReadOnlyDictionary<string, string> Rules
);