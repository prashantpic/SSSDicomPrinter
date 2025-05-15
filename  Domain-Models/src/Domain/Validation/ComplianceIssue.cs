namespace TheSSS.DicomViewer.Domain.Validation
{
    public sealed record ComplianceIssue
    {
        public string Description { get; }
        public IssueSeverity Severity { get; }

        public ComplianceIssue(string description, IssueSeverity severity)
        {
            Description = description;
            Severity = severity;
        }
    }
}