using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;
using TheSSS.DICOMViewer.Domain.Validation;

namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.Entities;

public class Series
{
    public SeriesInstanceUid SeriesInstanceUid { get; private set; }
    public string Modality { get; private set; }
    public int? SeriesNumber { get; private set; }
    public string? SeriesDescription { get; private set; }
    public string? BodyPartExamined { get; private set; }

    private readonly List<Instance> _instances = new();
    public IReadOnlyCollection<Instance> Instances => _instances.AsReadOnly();

    private Series(SeriesInstanceUid seriesInstanceUid, string modality, int? seriesNumber, string? seriesDescription, string? bodyPartExamined)
    {
        SeriesInstanceUid = seriesInstanceUid;
        Modality = modality;
        SeriesNumber = seriesNumber;
        SeriesDescription = seriesDescription;
        BodyPartExamined = bodyPartExamined;
    }

    public static Series Create(string seriesInstanceUid, string modality, int? seriesNumber, string? seriesDescription = null, string? bodyPartExamined = null)
    {
        var seriesUid = SeriesInstanceUid.Create(seriesInstanceUid);
        var series = new Series(seriesUid, modality, seriesNumber, seriesDescription, bodyPartExamined);
        
        var validator = new SeriesValidator();
        var result = validator.Validate(series);
        
        if (!result.IsValid)
        {
            throw new BusinessRuleViolationException($"Invalid series: {string.Join(", ", result.Errors)}");
        }
        
        return series;
    }

    public void AddInstance(Instance instance)
    {
        if (_instances.Any(i => i.SopInstanceUid.Equals(instance.SopInstanceUid)))
        {
            throw new BusinessRuleViolationException("Duplicate SOP Instance UID in series");
        }
        
        _instances.Add(instance);
    }

    public void RemoveInstance(SopInstanceUid sopInstanceUid)
    {
        var instance = _instances.FirstOrDefault(i => i.SopInstanceUid.Equals(sopInstanceUid));
        if (instance != null)
        {
            _instances.Remove(instance);
        }
    }
}