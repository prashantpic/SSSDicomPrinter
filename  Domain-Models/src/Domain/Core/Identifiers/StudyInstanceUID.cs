using System;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public record StudyInstanceUID(string Value)
    {
        public StudyInstanceUID() : this(string.Empty) { }
        
        public static StudyInstanceUID Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Study Instance UID cannot be empty");
            return new StudyInstanceUID(value);
        }
        
        public override string ToString() => Value;
    }
}