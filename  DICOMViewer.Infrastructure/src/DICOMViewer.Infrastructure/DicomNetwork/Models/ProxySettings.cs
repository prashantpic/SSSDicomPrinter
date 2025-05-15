namespace TheSSS.DICOMViewer.Infrastructure.DicomNetwork.Models
{
    public class ProxySettings
    {
        public bool UseProxy { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}