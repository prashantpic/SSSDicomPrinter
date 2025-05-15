using TheSSS.DicomViewer.Domain.Exceptions;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public sealed record SOPInstanceUID
    {
        public string Value { get; }

        public SOPInstanceUID(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("SOP Instance UID cannot be empty");
            
            Value = value;
        }

        public override string ToString() => Value;
        
        public bool Equals(SOPInstanceUID? other) => other != null && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}