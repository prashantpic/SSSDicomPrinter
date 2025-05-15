using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.Entities;
using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;
using TheSSS.DICOMViewer.Domain.Validation;

namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.Entities;

public class Series
{
    public SeriesInstanceUid Id { get; private set; }
    public string Modality { get; private set; }
    public int SeriesNumber { get; private set; }
    public string? SeriesDescription { get; private set; }
    public string? BodyPartExamined { get; private set; }
    private readonly List<Instance> _instances = new();
    public IReadOnlyCollection<Instance> Instances => _instances.AsReadOnly();

    private Series() { }

    public static Series Create(SeriesInstanceUid id, string modality, int seriesNumber, string? seriesDescription = null, string? bodyPartExamined = null)
    {
        var series = new Series
        {
            Id = id,
            Modality = modality,
            SeriesNumber = seriesNumber,
            SeriesDescription = seriesDescription,
            BodyPartExamined = bodyPartExamined
        };

        var validator = new SeriesValidator();
        validator.ValidateAndThrow(series);
        
        return series;
    }

    public void AddInstance(Instance instance)
    {
        _instances.Add(instance);
    }

    public void RemoveInstance(SopInstanceUid instanceId)
    {
        var instance = _instances.FirstOrDefault(i => i.Id == instanceId);
        if (instance != null)
        {
            _instances.Remove(instance);
        }
    }
}