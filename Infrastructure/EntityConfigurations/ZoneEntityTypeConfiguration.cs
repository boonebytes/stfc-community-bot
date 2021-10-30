using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
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
                .HasMaxLength(15)
                .IsRequired(true);

            zoneConfiguration.Property(z => z.DefendUtcTime)
                .HasMaxLength(10)
                .IsRequired(true);

            //zoneConfiguration
            //    .Ignore("Owner");

            zoneConfiguration
                .Ignore("_nextDefend");

            var starSystemNav = zoneConfiguration.Metadata.FindNavigation(nameof(Zone.StarSystems));
            starSystemNav.SetPropertyAccessMode(PropertyAccessMode.Field);

            

            var ownerNav = zoneConfiguration.Metadata.FindNavigation(nameof(Zone.Owner));
            ownerNav.SetPropertyAccessMode(PropertyAccessMode.Field);

            zoneConfiguration
                .Property<long?>("_ownerId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OwnerId")
                .IsRequired(false);

            zoneConfiguration.HasOne<Alliance>(z => z.Owner)
                .WithMany()
                .HasForeignKey("_ownerId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
