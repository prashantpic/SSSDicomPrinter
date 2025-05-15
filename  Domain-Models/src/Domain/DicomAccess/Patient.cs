using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Patient
    {
        private readonly List<Study> _studies = new List<Study>();

        public PatientId Id { get; private set; }
        public string PatientName { get; private set; }
        public IReadOnlyCollection<Study> Studies => _studies.AsReadOnly();

        private Patient() { }

        public Patient(PatientId id, string patientName)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            PatientName = patientName ?? throw new ArgumentNullException(nameof(patientName));
        }

        public void AddStudy(Study study)
        {
            if (study == null) throw new ArgumentNullException(nameof(study));
            if (study.PatientId != Id) throw new InvalidOperationException("Cannot add study from different patient");
            if (!_studies.Any(s => s.Id == study.Id))
            {
                _studies.Add(study);
            }
        }
    }
}