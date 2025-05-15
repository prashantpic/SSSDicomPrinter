namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class RolePermissionDbo
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public RoleDbo Role { get; set; }
        public PermissionDbo Permission { get; set; }
    }
}