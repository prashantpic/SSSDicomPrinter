using System.Collections.Generic;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Study
    {
        public StudyInstanceUID Id { get; init; }
        public PatientId PatientId { get; init; }
        public DateTime? StudyDate { get; private set; }
        public IReadOnlyCollection<Series> Series => _series.AsReadOnly();
        private readonly List<Series> _series = new();

        public Study(StudyInstanceUID id, PatientId patientId, DateTime? studyDate)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            PatientId = patientId ?? throw new ArgumentNullException(nameof(patientId));
            StudyDate = studyDate;
        }

        public void AddSeries(Series series)
        {
            if (series?.StudyId == null || !series.StudyId.Equals(Id))
                throw new ArgumentException("Invalid series assignment");
            
            _series.Add(series);
        }
    }
}