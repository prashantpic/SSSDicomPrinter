namespace TheSSS.DicomViewer.Domain.Validation
{
    public record ComplianceIssue(string Description, IssueSeverity Severity);
}