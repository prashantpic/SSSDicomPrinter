namespace DICOMViewer.UI.Coordinator.Models
{
    public class SyncParameters
    {
        public double ScrollOffsetX { get; set; }
        public double ScrollOffsetY { get; set; }
        public double ZoomFactor { get; set; }
        public string SourceViewId { get; set; }
        public SynchronizationType SyncType { get; set; }
    }

    public enum SynchronizationType { Scroll, Zoom, Pan, Other }
}