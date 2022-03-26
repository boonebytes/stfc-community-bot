using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class ZoneNeighbourEntityTypeConfiguration : IEntityTypeConfiguration<ZoneNeighbour>
    {
        public void Configure(EntityTypeBuilder<ZoneNeighbour> neighbourConfiguration)
        {
            neighbourConfiguration.ToTable("zone_neighbours");

            neighbourConfiguration.HasKey(zn => zn.Id);

            neighbourConfiguration.Property(zn => zn.Id)
                .ValueGeneratedOnAdd();

            
            var fromZoneNav = neighbourConfiguration.Metadata.FindNavigation(nameof(ZoneNeighbour.FromZone));
            fromZoneNav.SetPropertyAccessMode(PropertyAccessMode.Field);

            neighbourConfiguration
                .Property<long>("_fromZoneId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("from_zone_id")
                .IsRequired(true);

            neighbourConfiguration.HasOne<Zone>(zn => zn.FromZone)
                .WithMany(z => z.ZoneNeighbours)
                .HasForeignKey("_fromZoneId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
            

            var toZoneNav = neighbourConfiguration.Metadata.FindNavigation(nameof(ZoneNeighbour.ToZone));
            toZoneNav.SetPropertyAccessMode(PropertyAccessMode.Field);

            neighbourConfiguration
                .Property<long>("_toZoneId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("to_zone_id")
                .IsRequired(true);

            neighbourConfiguration.HasOne<Zone>(zn => zn.ToZone)
                .WithMany()
                .HasForeignKey("_toZoneId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
