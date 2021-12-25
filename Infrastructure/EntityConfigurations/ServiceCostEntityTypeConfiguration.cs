using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class ServiceCostEntityTypeConfiguration : IEntityTypeConfiguration<ServiceCost>
    {
        public void Configure(EntityTypeBuilder<ServiceCost> serviceCostConfiguration)
        {
            serviceCostConfiguration.ToTable("zone_service_cost");

            serviceCostConfiguration.HasKey(sc => sc.Id);

            serviceCostConfiguration.Property(sc => sc.Id)
                .ValueGeneratedOnAdd();
            
            serviceCostConfiguration.Property(sc => sc.Cost)
                .IsRequired(true);

            serviceCostConfiguration
                .Property<long?>("_serviceId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ServiceId")
                .IsRequired(true);

            serviceCostConfiguration.HasOne<Service>(sc => sc.Service)
                .WithMany(s => s.Costs)
                .HasForeignKey("_serviceId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
            
            var navigation = serviceCostConfiguration.Metadata.FindNavigation(nameof(ServiceCost.Service));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
