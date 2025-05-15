using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using TheSSS.DicomViewer.Common.Data;
using TheSSS.DicomViewer.Pdf.Models;

namespace TheSSS.DicomViewer.Pdf.Internal.Components
{
    internal class MetadataRenderComponent : IComponent
    {
        private readonly DicomMetadataCollection? _metadataCollection;
        private readonly DicomMetadataEmbeddingOptions? _embeddingOptions;

        public MetadataRenderComponent(DicomMetadataCollection? metadataCollection, DicomMetadataEmbeddingOptions? embeddingOptions)
        {
            _metadataCollection = metadataCollection;
            _embeddingOptions = embeddingOptions;
        }

        public void Compose(IContainer container)
        {
            if (_embeddingOptions?.Mode == MetadataEmbeddingMode.None || _metadataCollection == null)
                return;

            var tags = GetFilteredTags();
            if (!tags.Any()) return;

            container.Column(col =>
            {
                col.Spacing(5);
                col.Item().Text("DICOM Metadata").FontSize(10).Bold();
                foreach (var tag in tags)
                {
                    col.Item().Text($"{tag.Key}: {tag.Value}").FontSize(9);
                }
            });
        }

        private Dictionary<string, string> GetFilteredTags()
        {
            var filtered = new Dictionary<string, string>();
            var tagsToCheck = _embeddingOptions.Mode switch
            {
                MetadataEmbeddingMode.StandardSubset => _embeddingOptions.StandardSubsetKeys,
                MetadataEmbeddingMode.CustomList => _embeddingOptions.CustomTagsToEmbed,
                _ => null
            };

            if (tagsToCheck != null)
            {
                foreach (var tag in tagsToCheck)
                {
                    if (_metadataCollection.TryGetValue($"{tag.Group:X4}{tag.Element:X4}", out var value))
                    {
                        filtered[tag.Name] = value;
                    }
                }
            }
            return filtered;
        }
    }
}