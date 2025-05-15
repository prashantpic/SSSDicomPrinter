using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Study
    {
        private readonly List<Series> _series = new List<Series>();

        public StudyInstanceUID Id { get; private set; }
        public PatientId PatientId { get; private set; }
        public DateTime? StudyDate { get; private set; }
        public IReadOnlyCollection<Series> Series => _series.AsReadOnly();

        private Study() { }

        public Study(StudyInstanceUID id, PatientId patientId, DateTime? studyDate)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            PatientId = patientId ?? throw new ArgumentNullException(nameof(patientId));
            StudyDate = studyDate;
        }

        public void AddSeries(Series series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));
            if (series.StudyId != Id) throw new InvalidOperationException("Cannot add series from different study");
            if (!_series.Any(s => s.Id == series.Id))
            {
                _series.Add(series);
            }
        }
    }
}