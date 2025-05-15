using System.Collections.Generic;
using TheSSS.DicomViewer.Domain.Core.ValueObjects;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public class HeaderIntegrityRule : IComplianceRule
    {
        public IEnumerable<ComplianceIssue> Check(DicomValidationContext context)
        {
            var metaTags = context.FileMetadata.Keys.Where(t => t.Group == 0x0002).ToList();
            if (metaTags.Count == 0)
            {
                yield return new ComplianceIssue("Missing DICOM File Meta Information", IssueSeverity.Error);
            }
        }
    }
}