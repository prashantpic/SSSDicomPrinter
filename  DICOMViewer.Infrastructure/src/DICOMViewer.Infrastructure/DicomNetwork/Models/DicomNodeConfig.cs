namespace TheSSS.DICOMViewer.Infrastructure.DicomNetwork.Models
{
    public class DicomNodeConfig
    {
        public string Name { get; set; }
        public string AETitle { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseTls { get; set; }
        public List<string> AcceptedTransferSyntaxes { get; set; }
        public List<string> PreferredTransferSyntaxes { get; set; }
    }
}