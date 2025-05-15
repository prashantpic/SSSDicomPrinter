using System.Collections.Generic;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public class HeaderIntegrityRule : IComplianceRule
    {
        public IEnumerable<ComplianceIssue> Check(DicomValidationContext context)
        {
            if (context?.FileMetadata == null)
                yield return new ComplianceIssue("Invalid validation context", IssueSeverity.Error);
            
            // Basic header validation logic placeholder
            yield break;
        }
    }
}