using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Instance
    {
        public SOPInstanceUID Id { get; private set; }
        public SeriesInstanceUID SeriesId { get; private set; }
        public string InstanceNumber { get; private set; }

        private Instance() { }

        public Instance(SOPInstanceUID id, SeriesInstanceUID seriesId, string instanceNumber)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            SeriesId = seriesId ?? throw new ArgumentNullException(nameof(seriesId));
            InstanceNumber = instanceNumber;
        }
    }
}