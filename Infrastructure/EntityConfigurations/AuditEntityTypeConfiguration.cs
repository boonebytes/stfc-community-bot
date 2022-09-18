using DiscordBot.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class AuditEntityTypeConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> auditConfiguration)
        {
            auditConfiguration.ToTable("audit");

            auditConfiguration.HasKey(a => a.Id);
            auditConfiguration.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            auditConfiguration.Property(a => a.Type)
                .HasMaxLength(50);
            auditConfiguration.Property(a => a.TableName)
                .HasColumnName("table_name")
                .HasMaxLength(200);
            auditConfiguration.Property(a => a.DateTime)
                .HasColumnName("date_time");
            auditConfiguration.Property(a => a.OldValues)
                .HasColumnName("old_values");
            auditConfiguration.Property(a => a.NewValues)
                .HasColumnName("new_values");
            auditConfiguration.Property(a => a.AffectedColumns)
                .HasColumnName("affected_columns");
            auditConfiguration.Property(a => a.PrimaryKey)
                .HasColumnName("primary_key");
            auditConfiguration.Property(a => a.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(200);
        }
    }
}
