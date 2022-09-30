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
                .HasColumnName("zone_id")
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
