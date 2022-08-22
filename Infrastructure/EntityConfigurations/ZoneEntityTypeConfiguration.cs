using System;
using System.Collections.Generic;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations;

public class ZoneEntityTypeConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> zoneConfiguration)
    {
        zoneConfiguration.ToTable("zones");

        zoneConfiguration.HasKey(z => z.Id);

        zoneConfiguration.Property(z => z.Id)
            .ValueGeneratedOnAdd();

        zoneConfiguration.Property(z => z.Name)
            .HasMaxLength(200)
            .IsRequired(true);

        zoneConfiguration.Property(z => z.Threats)
            .HasMaxLength(2000);

        zoneConfiguration.Property(z => z.DefendUtcDayOfWeek)
            .HasColumnName("defend_day_of_week")
            .HasMaxLength(15)
            .IsRequired(true);

        zoneConfiguration.Property(z => z.DefendUtcTime)
            .HasColumnName("defend_utc_time")
            .HasMaxLength(10)
            .IsRequired(true);

        zoneConfiguration.Property(z => z.NextDefend)
            .HasColumnName("next_defend")
            .IsRequired(false);

        zoneConfiguration.Property(z => z.DefendEasternDay)
            .HasColumnName("defend_eastern_day");
        zoneConfiguration.Property(z => z.DefendEasternTime)
            .HasColumnName("defend_eastern_time");
        
        var starSystemNav = zoneConfiguration.Metadata.FindNavigation(nameof(Zone.StarSystems));
        starSystemNav.SetPropertyAccessMode(PropertyAccessMode.Field);
        
        zoneConfiguration
            .Ignore(z => z.Neighbours);

        zoneConfiguration
            .HasMany<ZoneNeighbour>(z => z.ZoneNeighbours)
            .WithOne(zn => zn.FromZone)
            .HasForeignKey("_fromZoneId")
            .OnDelete(DeleteBehavior.Restrict);
        

        var neighboursNav = zoneConfiguration.Metadata.FindNavigation(nameof(Zone.ZoneNeighbours));
        neighboursNav.SetPropertyAccessMode(PropertyAccessMode.Field);
        neighboursNav.SetIsEagerLoaded(true);
        
        var ownerNav = zoneConfiguration.Metadata.FindNavigation(nameof(Zone.Owner));
        ownerNav.SetPropertyAccessMode(PropertyAccessMode.Field);
        ownerNav.SetIsEagerLoaded(true);

        zoneConfiguration
            .Property<long?>("_ownerId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("owner_id")
            .IsRequired(false);

        zoneConfiguration.HasOne<Alliance>(z => z.Owner)
            .WithMany(a => a.Zones)
            .HasForeignKey("_ownerId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
