using FluentValidation;
using TheSSS.DICOMViewer.Domain.Exceptions;
using TheSSS.DICOMViewer.Domain.Validation;

namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;

public record SopInstanceUid(string Value)
{
    public static SopInstanceUid Create(string value)
    {
        var validator = new SopInstanceUidValidator();
        var result = validator.Validate(value);
        
        if (!result.IsValid)
        {
            throw new InvalidDicomIdentifierException(nameof(SopInstanceUid), value, result.Errors);
        }

        return new SopInstanceUid(value);
    }
    
    public override string ToString() => Value;
}