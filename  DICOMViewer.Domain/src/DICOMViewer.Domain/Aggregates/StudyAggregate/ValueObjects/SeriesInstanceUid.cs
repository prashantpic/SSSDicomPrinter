using FluentValidation;
using TheSSS.DICOMViewer.Domain.Exceptions;
using TheSSS.DICOMViewer.Domain.Validation;

namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;

public record SeriesInstanceUid(string Value)
{
    public static SeriesInstanceUid Create(string value)
    {
        var validator = new SeriesInstanceUidValidator();
        var result = validator.Validate(value);
        
        if (!result.IsValid)
        {
            throw new InvalidDicomIdentifierException(nameof(SeriesInstanceUid), value, result.Errors);
        }

        return new SeriesInstanceUid(value);
    }
    
    public override string ToString() => Value;
}