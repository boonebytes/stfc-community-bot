using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
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
                .HasMaxLength(200);
            auditConfiguration.Property(a => a.UserId)
                .HasMaxLength(200);
        }
    }
}
