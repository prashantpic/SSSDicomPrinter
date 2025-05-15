using FluentValidation;
using TheSSS.DICOMViewer.Domain.Validation;
using TheSSS.DICOMViewer.Domain.ValueObjects;

namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.Entities;

public class Instance
{
    public SopInstanceUid Id { get; private set; }
    public int InstanceNumber { get; private set; }
    public string SOPClassUID { get; private set; }
    public string PhotometricInterpretation { get; private set; }
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public PixelSpacing? PixelSpacing { get; private set; }

    private Instance() { }

    public static Instance Create(
        SopInstanceUid id,
        int instanceNumber,
        string sopClassUid,
        string photometricInterpretation,
        int rows,
        int columns,
        PixelSpacing? pixelSpacing = null)
    {
        var instance = new Instance
        {
            Id = id,
            InstanceNumber = instanceNumber,
            SOPClassUID = sopClassUid,
            PhotometricInterpretation = photometricInterpretation,
            Rows = rows,
            Columns = columns,
            PixelSpacing = pixelSpacing
        };

        var validator = new InstanceValidator();
        validator.ValidateAndThrow(instance);

        return instance;
    }
}