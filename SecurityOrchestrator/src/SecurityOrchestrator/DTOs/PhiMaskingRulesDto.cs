using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object containing a collection of rules for masking Protected Health Information (PHI).
    /// REQ-7-004
    /// </summary>
    /// <param name="Rules">A read-only dictionary where keys might represent field names or patterns, and values represent masking strategies or replacement text.</param>
    public record PhiMaskingRulesDto(
        IReadOnlyDictionary<string, string> Rules);
}