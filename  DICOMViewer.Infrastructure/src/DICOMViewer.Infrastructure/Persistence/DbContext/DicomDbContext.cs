using Microsoft.EntityFrameworkCore;
using TheSSS.DICOMViewer.Infrastructure.Persistence.Entities;

namespace TheSSS.DICOMViewer.Infrastructure.Persistence.DbContext
{
    public class DicomDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<PatientDbo> Patients { get; set; }
        public DbSet<StudyDbo> Studies { get; set; }
        public DbSet<SeriesDbo> Series { get; set; }
        public DbSet<InstanceDbo> Instances { get; set; }
        public DbSet<UserSettingDbo> UserSettings { get; set; }
        public DbSet<AuditLogDbo> AuditLogs { get; set; }
        public DbSet<PacsConfigurationDbo> PacsConfigurations { get; set; }
        public DbSet<AnonymizationProfileDbo> AnonymizationProfiles { get; set; }
        public DbSet<PixelAnonymizationTemplateDbo> PixelAnonymizationTemplates { get; set; }
        public DbSet<HangingProtocolDbo> HangingProtocols { get; set; }
        public DbSet<UserDbo> Users { get; set; }
        public DbSet<RoleDbo> Roles { get; set; }
        public DbSet<PermissionDbo> Permissions { get; set; }
        public DbSet<RolePermissionDbo> RolePermissions { get; set; }

        public DicomDbContext(DbContextOptions<DicomDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DicomDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}