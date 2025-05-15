namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class RolePermissionDbo
    {
        public Guid RoleId { get; set; }
        public RoleDbo Role { get; set; }
        public Guid PermissionId { get; set; }
        public PermissionDbo Permission { get; set; }
    }
}