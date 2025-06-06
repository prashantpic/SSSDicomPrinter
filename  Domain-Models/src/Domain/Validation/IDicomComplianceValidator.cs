using TheSSS.DicomViewer.Domain.Validation;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public interface IDicomComplianceValidator
    {
        ComplianceReport Validate(DicomValidationContext context);
    }
}