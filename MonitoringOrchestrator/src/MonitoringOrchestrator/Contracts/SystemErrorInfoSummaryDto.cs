namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class SystemErrorInfoSummaryDto
{
    public int CriticalErrorCountLast24Hours { get; set; }
    public List<ErrorTypeSummary> ErrorTypeSummaries { get; set; } = new();
}

public class ErrorTypeSummary
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
}