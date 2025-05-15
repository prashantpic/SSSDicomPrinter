using System.Collections.Generic;
using System.Linq;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public class DicomComplianceValidator : IDicomComplianceValidator
    {
        private readonly IEnumerable<IComplianceRule> _rules;

        public DicomComplianceValidator(IEnumerable<IComplianceRule> rules)
        {
            _rules = rules ?? Enumerable.Empty<IComplianceRule>();
        }

        public ComplianceReport Validate(DicomValidationContext context)
        {
            var issues = new List<ComplianceIssue>();
            
            foreach (var rule in _rules)
            {
                issues.AddRange(rule.Check(context));
            }

            return new ComplianceReport(!issues.Any(i => i.Severity == IssueSeverity.Error), issues);
        }
    }
}