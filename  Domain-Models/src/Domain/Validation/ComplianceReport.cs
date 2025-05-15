using System.Collections.Generic;
using System.Linq;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public sealed record ComplianceReport
    {
        public bool IsCompliant { get; }
        public IReadOnlyCollection<ComplianceIssue> Issues { get; }

        public ComplianceReport(bool isCompliant, IEnumerable<ComplianceIssue> issues)
        {
            IsCompliant = isCompliant;
            Issues = (issues ?? Enumerable.Empty<ComplianceIssue>()).ToList().AsReadOnly();
        }
    }
}