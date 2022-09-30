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
