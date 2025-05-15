using System.Collections.Generic;
using TheSSS.DicomViewer.Domain.Core.ValueObjects;

namespace TheSSS.DicomViewer.Domain.Validation
{
    public record DicomValidationContext(IReadOnlyDictionary<DicomTag, string> FileMetadata);
}