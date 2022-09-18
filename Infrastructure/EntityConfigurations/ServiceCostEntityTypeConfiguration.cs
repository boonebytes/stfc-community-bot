using DiscordBot.Domain.Entities.Services;
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

            serviceCostConfiguration
                .Property<int?>("_resourceId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("resource_id");
            serviceCostConfiguration.HasOne(sc => sc.Resource)
                .WithMany()
                .HasForeignKey("_resourceId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            serviceCostConfiguration.Property(sc => sc.Cost)
                .IsRequired(true);

            serviceCostConfiguration
                .Property<long?>("_serviceId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("service_id")
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
