using System.Text.Json.Serialization;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Models
{
    public class SyncParameters
    {
        [JsonPropertyName("syncType")]
        public SynchronizationType SyncType { get; set; }
        
        [JsonPropertyName("scrollOffsetX")]
        public double ScrollOffsetX { get; set; }
        
        [JsonPropertyName("scrollOffsetY")]
        public double ScrollOffsetY { get; set; }
        
        [JsonPropertyName("zoomFactor")]
        public double ZoomFactor { get; set; }
    }
}