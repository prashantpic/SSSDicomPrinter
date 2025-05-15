using System.Collections.Generic;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public record DicomComplianceProfile(string Name, IReadOnlyCollection<IComplianceRule> Rules);
}