namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class AnonymizationProfileDbo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RulesJson { get; set; }
        public bool IsReadOnly { get; set; }
    }
}