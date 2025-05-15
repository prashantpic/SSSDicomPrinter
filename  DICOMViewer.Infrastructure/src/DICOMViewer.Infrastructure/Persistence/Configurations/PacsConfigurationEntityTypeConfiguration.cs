using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheSSS.DICOMViewer.Infrastructure.Persistence.Entities;

namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Configurations
{
    public class PacsConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<PacsConfigurationDbo>
    {
        public void Configure(EntityTypeBuilder<PacsConfigurationDbo> builder)
        {
            builder.ToTable("PACSConfigurations");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.AeTitle).IsRequired().HasMaxLength(16);
            builder.Property(p => p.Host).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Port).IsRequired();
            builder.HasIndex(p => p.AeTitle).IsUnique();
        }
    }
}