using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class ServiceEntityTypeConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> serviceConfiguration)
        {
            serviceConfiguration.ToTable("zone_services");

            serviceConfiguration.HasKey(s => s.Id);

            serviceConfiguration.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            serviceConfiguration.Property(s => s.Name)
                .HasMaxLength(200)
                .IsRequired(true);

            serviceConfiguration
                .Property<long?>("_zoneId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ZoneId")
                .IsRequired(true);

            serviceConfiguration.HasOne<Zone>(s => s.Zone)
                .WithMany(z => z.Services)
                .HasForeignKey("_zoneId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
            
            serviceConfiguration
                .HasMany<ServiceCost>(s => s.Costs)
                .WithOne(sc => sc.Service)
                .HasForeignKey("_serviceId")
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = serviceConfiguration.Metadata.FindNavigation(nameof(Service.Zone));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
