namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class InstanceDbo
    {
        public int Id { get; set; }
        public string SopInstanceUid { get; set; }
        public string SopClassUid { get; set; }
        public int? InstanceNumber { get; set; }
        public string FilePath { get; set; }
        public int SeriesId { get; set; }
        public SeriesDbo Series { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}