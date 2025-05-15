using TheSSS.DICOMViewer.Domain.Validation;
using TheSSS.DICOMViewer.Domain.Exceptions;

namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate;

public class Study
{
    public StudyInstanceUid StudyInstanceUid { get; private set; }
    public PatientId PatientId { get; private set; }
    public DateTime? StudyDate { get; private set; }
    public string? AccessionNumber { get; private set; }
    public string? StudyDescription { get; private set; }
    private readonly List<Series> _series = new();
    public IReadOnlyCollection<Series> Series => _series.AsReadOnly();

    private Study(StudyInstanceUid studyInstanceUid, PatientId patientId, DateTime? studyDate, string? accessionNumber, string? studyDescription)
    {
        StudyInstanceUid = studyInstanceUid;
        PatientId = patientId;
        StudyDate = studyDate;
        AccessionNumber = accessionNumber;
        StudyDescription = studyDescription;
    }

    public static Study Create(string studyInstanceUid, PatientId patientId, DateTime? studyDate, string? accessionNumber, string? studyDescription)
    {
        var studyUid = StudyInstanceUid.Create(studyInstanceUid);
        var study = new Study(studyUid, patientId, studyDate, accessionNumber, studyDescription);
        
        var validator = new StudyValidator();
        var result = validator.Validate(study);
        
        if (!result.IsValid)
        {
            throw new BusinessRuleViolationException($"Study validation failed: {string.Join(", ", result.Errors)}");
        }

        return study;
    }

    public void AddSeries(Series series)
    {
        if (_series.Any(s => s.SeriesInstanceUid.Equals(series.SeriesInstanceUid)))
            throw new BusinessRuleViolationException("Series with same UID already exists in study");

        _series.Add(series);
        UpdateModalities();
    }

    private void UpdateModalities()
    {
        var modalities = _series.Select(s => s.Modality)
            .Distinct()
            .OrderBy(m => m);
    }
}