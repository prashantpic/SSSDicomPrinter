using System.Collections.Generic;
using System.Linq;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public record ComplianceReport(IReadOnlyCollection<ComplianceIssue> Issues)
    {
        public bool IsCompliant => !Issues.Any(i => i.Severity == IssueSeverity.Error);
    }
}