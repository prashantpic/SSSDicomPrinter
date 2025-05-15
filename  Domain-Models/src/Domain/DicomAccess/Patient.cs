using System.Collections.Generic;
using System.Collections.ObjectModel;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Patient
    {
        private readonly List<Study> _studies = new();
        
        public PatientId Id { get; }
        public string PatientName { get; private set; }
        public IReadOnlyCollection<Study> Studies => new ReadOnlyCollection<Study>(_studies);

        public Patient(PatientId id, string patientName)
        {
            Id = id;
            PatientName = patientName;
        }

        public void AddStudy(Study study)
        {
            if (study.PatientId != Id)
                throw new InvalidOperationException("Study belongs to different patient");
            _studies.Add(study);
        }
    }
}