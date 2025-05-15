namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class RoleDbo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<RolePermissionDbo> RolePermissions { get; set; }
    }
}