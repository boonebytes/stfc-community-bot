using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class StarSystemEntityTypeConfiguration : IEntityTypeConfiguration<StarSystem>
    {
        public void Configure(EntityTypeBuilder<StarSystem> starSystemConfiguration)
        {
            starSystemConfiguration.ToTable("starsystems");

            starSystemConfiguration.HasKey(ss => ss.Id);

            starSystemConfiguration.Property(ss => ss.Id)
                .ValueGeneratedOnAdd();

            starSystemConfiguration.Property(ss => ss.Name)
                .HasMaxLength(200)
                .IsRequired(true);

            starSystemConfiguration
                .Property<long?>("_zoneId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("zone_id")
                .IsRequired(true);

            starSystemConfiguration.HasOne<Zone>(ss => ss.Zone)
                .WithMany(z => z.StarSystems)
                .HasForeignKey("_zoneId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            starSystemConfiguration
                .Property<int?>("_resourceId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("resource_id")
                .IsRequired(false);

            starSystemConfiguration.HasOne<Resource>()
                .WithMany()
                .HasForeignKey("_resourceId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            var navigation = starSystemConfiguration.Metadata.FindNavigation(nameof(StarSystem.Zone));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
