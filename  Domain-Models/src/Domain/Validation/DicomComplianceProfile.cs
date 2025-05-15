using System.Collections.Generic;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public sealed record DicomComplianceProfile
    {
        public string Name { get; }
        public IReadOnlyCollection<IComplianceRule> Rules { get; }

        public DicomComplianceProfile(string name, IEnumerable<IComplianceRule> rules)
        {
            Name = name;
            Rules = (rules ?? Enumerable.Empty<IComplianceRule>()).ToList().AsReadOnly();
        }
    }
}