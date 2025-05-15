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
            builder.HasIndex(p => p.Name).IsUnique();
            
            builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
            builder.Property(p => p.AETitle).IsRequired().HasMaxLength(16);
            builder.Property(p => p.Host).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Port).IsRequired();
            builder.Property(p => p.EncryptedPassword).HasColumnType("TEXT");
            builder.Property(p => p.IsDefault).IsRequired().HasDefaultValue(false);
            builder.Property(p => p.ProxyType).HasMaxLength(20);
            builder.Property(p => p.ProxyHost).HasMaxLength(255);
            builder.Property(p => p.ProxyPort);
            builder.Property(p => p.ProxyUsername).HasMaxLength(255);
            builder.Property(p => p.EncryptedProxyPassword).HasColumnType("TEXT");
        }
    }
}