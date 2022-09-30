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

using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class AllianceServiceEntityTypeConfiguration : IEntityTypeConfiguration<AllianceService>
    {
        public void Configure(EntityTypeBuilder<AllianceService> allianceServiceConfiguration)
        {
            allianceServiceConfiguration.ToTable("alliance_services");

            allianceServiceConfiguration.HasKey(sc => sc.Id);

            allianceServiceConfiguration.Property(sc => sc.Id)
                .ValueGeneratedOnAdd();
            
            allianceServiceConfiguration
                .Property<long?>("_allianceId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("alliance_id")
                .IsRequired(true);

            allianceServiceConfiguration.HasOne<Alliance>(s => s.Alliance)
                .WithMany(a => a.AllianceServices)
                .HasForeignKey("_allianceId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
            

            allianceServiceConfiguration
                .Property<long?>("_serviceId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("zone_service_id")
                .IsRequired(true);

            allianceServiceConfiguration.HasOne<Service>(s => s.Service)
                .WithMany(s => s.AllianceServices)
                .HasForeignKey("_serviceId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
            
            
            allianceServiceConfiguration
                .Property<int?>("_allianceServiceLevelId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("level_id");
            
            allianceServiceConfiguration.HasOne(s => s.AllianceServiceLevel)
                .WithMany()
                .HasForeignKey("_allianceServiceLevelId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            
            var navigation = allianceServiceConfiguration.Metadata.FindNavigation(nameof(ServiceCost.Service));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
