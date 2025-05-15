using System.Collections.Generic;
using TheSSS.DicomViewer.Common.Data;

namespace TheSSS.DicomViewer.Pdf.Models
{
    public class DicomMetadataEmbeddingOptions
    {
        public MetadataEmbeddingMode Mode { get; set; }
        public IReadOnlyList<DicomTagInfo>? StandardSubsetKeys { get; set; }
        public IReadOnlyList<DicomTagInfo>? CustomTagsToEmbed { get; set; }
    }
}