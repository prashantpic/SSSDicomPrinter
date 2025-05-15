using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Instance
    {
        public SOPInstanceUID Id { get; init; }
        public SeriesInstanceUID SeriesId { get; init; }
        public string InstanceNumber { get; private set; }

        public Instance(SOPInstanceUID id, SeriesInstanceUID seriesId, string instanceNumber)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            SeriesId = seriesId ?? throw new ArgumentNullException(nameof(seriesId));
            InstanceNumber = instanceNumber ?? throw new ArgumentNullException(nameof(instanceNumber));
        }
    }
}