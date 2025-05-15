namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class PacsConfigurationDbo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AETitle { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string EncryptedPassword { get; set; }
        public bool IsDefault { get; set; }
        public string ProxyType { get; set; }
        public string ProxyHost { get; set; }
        public int? ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string EncryptedProxyPassword { get; set; }
    }
}