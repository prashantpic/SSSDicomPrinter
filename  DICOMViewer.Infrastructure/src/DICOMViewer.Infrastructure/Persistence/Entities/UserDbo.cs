namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class UserDbo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public ICollection<RolePermissionDbo> RolePermissions { get; set; }
    }
}