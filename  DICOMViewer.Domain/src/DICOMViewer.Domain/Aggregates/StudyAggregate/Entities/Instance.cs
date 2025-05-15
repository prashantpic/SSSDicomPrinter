using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;
using TheSSS.DICOMViewer.Domain.ValueObjects;

namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.Entities;

public class Instance
{
    public SopInstanceUid SopInstanceUid { get; private set; }
    public int? InstanceNumber { get; private set; }
    public string SOPClassUID { get; private set; }
    public string PhotometricInterpretation { get; private set; }
    public ushort Rows { get; private set; }
    public ushort Columns { get; private set; }
    public PixelSpacing? PixelSpacing { get; private set; }

    private Instance(SopInstanceUid sopInstanceUid, int? instanceNumber, string sopClassUid, 
                   string photometricInterpretation, ushort rows, ushort columns, PixelSpacing? pixelSpacing)
    {
        SopInstanceUid = sopInstanceUid;
        InstanceNumber = instanceNumber;
        SOPClassUID = sopClassUid;
        PhotometricInterpretation = photometricInterpretation;
        Rows = rows;
        Columns = columns;
        PixelSpacing = pixelSpacing;
    }

    public static Instance Create(string sopInstanceUid, string sopClassUid, string photometricInterpretation,
                                ushort rows, ushort columns, PixelSpacing? pixelSpacing, int? instanceNumber = null)
    {
        var sopUid = SopInstanceUid.Create(sopInstanceUid);
        var instance = new Instance(sopUid, instanceNumber, sopClassUid, photometricInterpretation, rows, columns, pixelSpacing);
        
        var validator = new InstanceValidator();
        var result = validator.Validate(instance);
        
        if (!result.IsValid)
        {
            throw new BusinessRuleViolationException($"Invalid instance: {string.Join(", ", result.Errors)}");
        }
        
        return instance;
    }
}