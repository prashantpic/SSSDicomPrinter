using System.Collections.Generic;
using System.Collections.ObjectModel;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Study
    {
        private readonly List<Series> _series = new();
        
        public StudyInstanceUID Id { get; }
        public PatientId PatientId { get; }
        public IReadOnlyCollection<Series> Series => new ReadOnlyCollection<Series>(_series);

        public Study(StudyInstanceUID id, PatientId patientId)
        {
            Id = id;
            PatientId = patientId;
        }

        public void AddSeries(Series series)
        {
            if (series.StudyId != Id)
                throw new InvalidOperationException("Series belongs to different study");
            _series.Add(series);
        }
    }
}