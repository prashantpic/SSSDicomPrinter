using System;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public record SeriesInstanceUID(string Value)
    {
        public SeriesInstanceUID() : this(string.Empty) { }
        
        public static SeriesInstanceUID Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Series Instance UID cannot be empty");
            return new SeriesInstanceUID(value);
        }
        
        public override string ToString() => Value;
    }
}