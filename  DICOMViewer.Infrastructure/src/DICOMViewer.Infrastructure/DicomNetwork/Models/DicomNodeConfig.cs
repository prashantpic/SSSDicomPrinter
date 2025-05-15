namespace TheSSS.DICOMViewer.Infrastructure.DicomNetwork.Models
{
    public class DicomNodeConfig
    {
        public string Name { get; set; }
        public string AeTitle { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string PeerAeTitle { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> SupportedTransferSyntaxes { get; set; }
        public ProxySettings ProxySettings { get; set; }
    }
}