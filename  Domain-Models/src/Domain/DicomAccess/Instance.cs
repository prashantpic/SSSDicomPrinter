using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Instance
    {
        public SOPInstanceUID Id { get; }
        public SeriesInstanceUID SeriesId { get; }

        public Instance(SOPInstanceUID id, SeriesInstanceUID seriesId)
        {
            Id = id;
            SeriesId = seriesId;
        }
    }
}