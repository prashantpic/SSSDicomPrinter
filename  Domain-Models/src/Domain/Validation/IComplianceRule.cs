using System.Collections.Generic;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public interface IComplianceRule
    {
        IEnumerable<ComplianceIssue> Check(DicomValidationContext context);
    }
}