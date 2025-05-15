using System.Collections.Generic;
using System.Linq;
using TheSSS.DicomViewer.Domain.Core.ValueObjects;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public class RequiredTagsRule : IComplianceRule
    {
        private readonly IReadOnlyCollection<DicomTag> _requiredTags;

        public RequiredTagsRule(IEnumerable<DicomTag> requiredTags)
        {
            _requiredTags = requiredTags?.ToList().AsReadOnly() ?? new List<DicomTag>().AsReadOnly();
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