using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class AllianceServiceLevelEntityTypeConfiguration : IEntityTypeConfiguration<AllianceServiceLevel>
    {
        public void Configure(EntityTypeBuilder<AllianceServiceLevel> serviceLevelConfigu)
        {
            serviceLevelConfigu.ToTable("ct_alliance_service_level");

            serviceLevelConfigu.HasKey(r => r.Id);

            serviceLevelConfigu.Property(r => r.Id)
                .HasDefaultValue(0)
                .ValueGeneratedNever()
                .IsRequired();

            serviceLevelConfigu.Property(r => r.Name)
                .HasMaxLength(200)
                .IsRequired(true);

            serviceLevelConfigu.Property(r => r.Label)
                .HasMaxLength(200)
                .IsRequired(true);

            serviceLevelConfigu.HasData(
                data: AllianceServiceLevel.List()
            );
        }
    }
}
