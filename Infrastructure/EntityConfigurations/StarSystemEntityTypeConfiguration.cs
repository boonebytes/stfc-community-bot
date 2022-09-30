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
