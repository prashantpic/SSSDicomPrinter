using TheSSS.DicomViewer.Domain.Exceptions;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public sealed record SeriesInstanceUID
    {
        public string Value { get; }

        public SeriesInstanceUID(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Series Instance UID cannot be empty");
            
            Value = value;
        }

        public override string ToString() => Value;
        
        public bool Equals(SeriesInstanceUID? other) => other != null && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}