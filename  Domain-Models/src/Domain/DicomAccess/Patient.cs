using System.Collections.Generic;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Patient
    {
        public PatientId Id { get; init; }
        public string PatientName { get; private set; }
        public IReadOnlyCollection<Study> Studies => _studies.AsReadOnly();
        private readonly List<Study> _studies = new();

        public Patient(PatientId id, string patientName)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            PatientName = patientName ?? throw new ArgumentNullException(nameof(patientName));
        }

        public void AddStudy(Study study)
        {
            if (study?.PatientId == null || !study.PatientId.Equals(Id))
                throw new ArgumentException("Invalid study assignment");
            
            _studies.Add(study);
        }
    }
}