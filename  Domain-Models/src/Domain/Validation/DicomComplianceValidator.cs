using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public class DicomComplianceValidator : IDicomComplianceValidator
    {
        private readonly IReadOnlyCollection<IComplianceRule> _rules;

        public DicomComplianceValidator(IEnumerable<IComplianceRule> rules)
        {
            _rules = new ReadOnlyCollection<IComplianceRule>(rules?.ToList() ?? new List<IComplianceRule>());
        }

        public ComplianceReport Validate(DicomValidationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var issues = new List<ComplianceIssue>();
            foreach (var rule in _rules)
            {
                issues.AddRange(rule.Check(context));
            }
            return new ComplianceReport(!issues.Any(i => i.Severity == IssueSeverity.Error), issues.AsReadOnly());
        }
    }
}