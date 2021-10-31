using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class AllianceGroupEntityTypeConfiguration : IEntityTypeConfiguration<AllianceGroup>
    {
        public void Configure(EntityTypeBuilder<AllianceGroup> allianceGroupConfiguration)
        {
            allianceGroupConfiguration.ToTable("alliance_groups");

            allianceGroupConfiguration.HasKey(a => a.Id);
            allianceGroupConfiguration.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            allianceGroupConfiguration.Property(a => a.Name)
                .HasMaxLength(200);


            allianceGroupConfiguration.HasMany<Alliance>(ag => ag.Alliances)
                .WithOne(a => a.Group)
                .OnDelete(DeleteBehavior.Restrict);

            var navigation = allianceGroupConfiguration.Metadata.FindNavigation(nameof(AllianceGroup.Alliances));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
