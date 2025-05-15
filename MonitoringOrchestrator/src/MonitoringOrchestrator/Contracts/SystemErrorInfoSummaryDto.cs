namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class SystemErrorInfoSummaryDto
{
    public int CriticalErrorCountLast24Hours { get; set; }
    public IEnumerable<ErrorTypeSummary> ErrorTypeSummaries { get; set; } = Enumerable.Empty<ErrorTypeSummary>();
}

public class ErrorTypeSummary
{
    public string Type { get; set; } = string.Empty; // e.g., Exception type name
    public int Count { get; set; }
}