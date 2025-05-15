using System.Collections.Generic;
using TheSSS.DicomViewer.Domain.Core.ValueObjects;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public sealed record DicomValidationContext
    {
        public IReadOnlyDictionary<DicomTag, string> FileMetadata { get; }

        public DicomValidationContext(IReadOnlyDictionary<DicomTag, string> fileMetadata)
        {
            FileMetadata = fileMetadata ?? new Dictionary<DicomTag, string>();
        }
    }
}