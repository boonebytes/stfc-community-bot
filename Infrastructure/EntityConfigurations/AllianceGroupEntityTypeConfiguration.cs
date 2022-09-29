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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class AllianceGroupEntityTypeConfiguration : IEntityTypeConfiguration<AllianceGroup>
    {
        public void Configure(EntityTypeBuilder<AllianceGroup> allianceGroupConfiguration)
        {
            allianceGroupConfiguration.ToTable("alliance_groups");

            allianceGroupConfiguration.HasKey(a => a.Id);
            allianceGroupConfiguration.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            allianceGroupConfiguration.Property(a => a.Name)
                .HasMaxLength(200);


            allianceGroupConfiguration.HasMany<Alliance>(ag => ag.Alliances)
                .WithOne(a => a.Group)
                .OnDelete(DeleteBehavior.Restrict);

            var navigation = allianceGroupConfiguration.Metadata.FindNavigation(nameof(AllianceGroup.Alliances));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
