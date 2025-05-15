using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class SystemErrorInfoSummaryDto
{
    /// <summary>
    /// The lookback period for which this summary applies.
    /// </summary>
    public TimeSpan LookbackPeriod { get; set; }

    /// <summary>
    /// Count of critical errors logged within the lookback period.
    /// </summary>
    public int CriticalErrorCountTotal { get; set; } // Renamed from CriticalErrorCountLast24Hours for clarity with LookbackPeriod

    /// <summary>
    /// Breakdown of critical error counts by type or category.
    /// </summary>
    public List<ErrorTypeSummary> ErrorTypeSummaries { get; set; } = new List<ErrorTypeSummary>();
}

public class ErrorTypeSummary
{
    /// <summary>
    /// Type or category of the error (e.g., "DatabaseException", "NetworkError").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Count of errors of this type within the lookback period.
    /// </summary>
    public int Count { get; set; }
}