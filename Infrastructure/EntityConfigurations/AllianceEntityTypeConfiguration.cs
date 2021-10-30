using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class AllianceEntityTypeConfiguration : IEntityTypeConfiguration<Alliance>
    {
        public void Configure(EntityTypeBuilder<Alliance> allianceConfiguration)
        {
            allianceConfiguration.ToTable("alliances");

            allianceConfiguration.HasKey(a => a.Id);
            allianceConfiguration.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            allianceConfiguration.Property(a => a.Name)
                .HasMaxLength(200);

            allianceConfiguration.Property(a => a.Acronym)
                .HasMaxLength(5);

            allianceConfiguration.Property(a => a.DefendSchedulePostTime)
                .HasMaxLength(10);

            allianceConfiguration.HasMany<Diplomacy>(a => a.Diplomacy)
                .WithOne(d => d.Owner)
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = allianceConfiguration.Metadata.FindNavigation(nameof(Alliance.Diplomacy));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
