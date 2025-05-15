using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Models
{
    public class ViewState
    {
        [JsonPropertyName("stateData")]
        public Dictionary<string, object> StateData { get; set; } = new Dictionary<string, object>();
    }
}