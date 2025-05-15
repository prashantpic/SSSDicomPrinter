using System.Collections.Generic;
using System.Linq;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public class RequiredTagsRule : IComplianceRule
    {
        private readonly HashSet<DicomTag> _requiredTags;

        public RequiredTagsRule(IEnumerable<DicomTag> requiredTags)
        {
            _requiredTags = new HashSet<DicomTag>(requiredTags ?? Enumerable.Empty<DicomTag>());
        }

        public IEnumerable<ComplianceIssue> Check(DicomValidationContext context)
        {
            foreach (var tag in _requiredTags)
            {
                if (!context.FileMetadata.ContainsKey(tag))
                {
                    yield return new ComplianceIssue($"Missing required tag: {tag}", IssueSeverity.Error);
                }
            }
        }
    }
}