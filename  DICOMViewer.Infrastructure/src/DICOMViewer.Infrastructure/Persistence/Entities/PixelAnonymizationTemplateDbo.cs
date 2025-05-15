namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class PixelAnonymizationTemplateDbo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AreaDefinitionsJson { get; set; }
    }
}