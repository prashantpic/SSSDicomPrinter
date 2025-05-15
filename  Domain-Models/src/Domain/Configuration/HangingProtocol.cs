namespace TheSSS.DicomViewer.Domain.Configuration
{
    public class HangingProtocol
    {
        public HangingProtocolId Id { get; init; }
        public string ProtocolName { get; private set; }
        public HangingProtocolLayout LayoutConfiguration { get; private set; }

        public HangingProtocol(HangingProtocolId id, string protocolName, HangingProtocolLayout layout)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ProtocolName = protocolName ?? throw new ArgumentNullException(nameof(protocolName));
            LayoutConfiguration = layout ?? throw new ArgumentNullException(nameof(layout));
        }
    }
}