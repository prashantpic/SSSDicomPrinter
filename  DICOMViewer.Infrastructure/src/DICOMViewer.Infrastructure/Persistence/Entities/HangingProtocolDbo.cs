namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class HangingProtocolDbo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProtocolData { get; set; }
        public string UserIdentifier { get; set; }
        public string ModalityFilter { get; set; }
    }
}