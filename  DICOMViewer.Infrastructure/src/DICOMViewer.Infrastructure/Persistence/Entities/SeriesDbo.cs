namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class SeriesDbo
    {
        public int Id { get; set; }
        public string SeriesInstanceUid { get; set; }
        public string Modality { get; set; }
        public int? SeriesNumber { get; set; }
        public string SeriesDescription { get; set; }
        public int StudyId { get; set; }
        public StudyDbo Study { get; set; }
        public ICollection<InstanceDbo> Instances { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}