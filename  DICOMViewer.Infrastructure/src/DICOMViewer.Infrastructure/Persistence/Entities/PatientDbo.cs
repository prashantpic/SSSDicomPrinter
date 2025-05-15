namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class PatientDbo
    {
        public int Id { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Sex { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public ICollection<StudyDbo> Studies { get; set; }
    }
}