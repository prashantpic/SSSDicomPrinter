namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class UserDbo
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; }
        public ICollection<RolePermissionDbo> RolePermissions { get; set; }
    }
}