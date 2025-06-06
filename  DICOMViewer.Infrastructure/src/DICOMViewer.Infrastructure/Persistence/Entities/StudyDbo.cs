namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class StudyDbo
    {
        public int Id { get; set; }
        public string StudyInstanceUid { get; set; }
        public DateTime? StudyDate { get; set; }
        public TimeSpan? StudyTime { get; set; }
        public string AccessionNumber { get; set; }
        public string StudyDescription { get; set; }
        public int PatientId { get; set; }
        public PatientDbo Patient { get; set; }
        public ICollection<SeriesDbo> Series { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}