namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class PacsConfigurationDbo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AeTitle { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string PeerAeTitle { get; set; }
        public string Password { get; set; }
        public string SupportedTransferSyntaxesCsv { get; set; }
        public bool IsDefault { get; set; }
    }
}