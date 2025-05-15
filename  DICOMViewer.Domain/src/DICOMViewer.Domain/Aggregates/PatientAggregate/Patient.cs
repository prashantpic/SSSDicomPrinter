using TheSSS.DICOMViewer.Domain.Validation;
using TheSSS.DICOMViewer.Domain.Exceptions;

namespace TheSSS.DICOMViewer.Domain.Aggregates.PatientAggregate;

public class Patient
{
    public PatientId PatientId { get; private set; }
    public string PatientName { get; private set; }
    public DateTime? PatientBirthDate { get; private set; }
    public string? PatientSex { get; private set; }

    private Patient(PatientId patientId, string patientName, DateTime? patientBirthDate, string? patientSex)
    {
        PatientId = patientId;
        PatientName = patientName;
        PatientBirthDate = patientBirthDate;
        PatientSex = patientSex;
    }

    public static Patient Create(string patientId, string patientName, DateTime? patientBirthDate, string? patientSex)
    {
        var patientIdValue = PatientId.Create(patientId);
        var patient = new Patient(patientIdValue, patientName, patientBirthDate, patientSex);
        
        var validator = new PatientValidator();
        var result = validator.Validate(patient);
        
        if (!result.IsValid)
        {
            throw new BusinessRuleViolationException($"Patient validation failed: {string.Join(", ", result.Errors)}");
        }

        return patient;
    }

    public void UpdateDetails(string patientName, DateTime? patientBirthDate, string? patientSex)
    {
        PatientName = patientName;
        PatientBirthDate = patientBirthDate;
        PatientSex = patientSex;

        var validator = new PatientValidator();
        var result = validator.Validate(this);
        
        if (!result.IsValid)
        {
            throw new BusinessRuleViolationException($"Patient update validation failed: {string.Join(", ", result.Errors)}");
        }
    }

    public bool IsAdult()
    {
        if (!PatientBirthDate.HasValue) return false;
        return DateTime.Today.AddYears(-18) >= PatientBirthDate.Value.Date;
    }
}