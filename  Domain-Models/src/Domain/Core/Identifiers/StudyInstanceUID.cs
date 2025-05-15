using TheSSS.DicomViewer.Domain.Exceptions;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public sealed record StudyInstanceUID
    {
        public string Value { get; }

        public StudyInstanceUID(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Study Instance UID cannot be empty");
            
            Value = value;
        }

        public override string ToString() => Value;
        
        public bool Equals(StudyInstanceUID? other) => other != null && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}