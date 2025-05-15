using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Configuration
{
    public class HangingProtocol
    {
        public HangingProtocolId Id { get; private set; }
        public string ProtocolName { get; private set; }
        public object LayoutConfiguration { get; private set; }

        private HangingProtocol() { }

        public HangingProtocol(HangingProtocolId id, string protocolName, object layoutConfiguration)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ProtocolName = protocolName ?? throw new ArgumentNullException(nameof(protocolName));
            LayoutConfiguration = layoutConfiguration ?? throw new ArgumentNullException(nameof(layoutConfiguration));
        }
    }
}