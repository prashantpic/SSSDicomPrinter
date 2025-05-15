using TheSSS.DicomViewer.Domain.Exceptions;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public sealed record PatientId
    {
        public string Value { get; }

        public PatientId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Patient ID cannot be empty");
            
            Value = value;
        }

        public override string ToString() => Value;
        
        public bool Equals(PatientId? other) => other != null && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}