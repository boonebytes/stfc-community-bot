/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
