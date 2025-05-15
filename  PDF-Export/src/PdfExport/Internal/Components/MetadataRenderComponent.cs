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

            var tags = _embeddingOptions.Mode switch
            {
                MetadataEmbeddingMode.StandardSubset => _embeddingOptions.StandardSubsetKeys,
                MetadataEmbeddingMode.CustomList => _embeddingOptions.CustomTagsToEmbed,
                _ => null
            };

            if (tags?.Any() != true) return;

            container.Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("DICOM Metadata").Bold().FontSize(10);
                
                foreach (var tag in tags)
                {
                    if (_metadataCollection.TryGetValue($"{tag.Group:X4}{tag.Element:X4}", out var value))
                        column.Item().Text($"{tag.Name}: {value}").FontSize(9);
                }
            });
        }
    }
}