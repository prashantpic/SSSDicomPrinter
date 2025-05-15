using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Represents a summary of critical system errors.
/// </summary>
/// <param name="CriticalErrorCountLast24Hours">The total count of critical errors in the last 24 hours.</param>
/// <param name="ErrorTypeSummaries">A list summarizing errors by type.</param>
public record SystemErrorInfoSummaryDto(
    int CriticalErrorCountLast24Hours,
    List<ErrorTypeSummary> ErrorTypeSummaries
);

/// <summary>
/// Represents a summary for a specific type of error.
/// </summary>
/// <param name="Type">The type or category of the error.</param>
/// <param name="Count">The number of occurrences of this error type.</param>
public record ErrorTypeSummary(
    string Type,
    int Count
);